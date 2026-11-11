using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.BusinessRules;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.Composers;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.CoreDto;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.DbQueries;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.Loaders;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Requests.RequestDto;

namespace Modsen.Portal.Identity.Requests.PermissionAssignments.Requests;

internal sealed class UpdateUserPermissionsRequest
    : ICommandHandler<UpdateUserPermissionsRequest.Command, List<PermissionAssignmentDto>>
{
    private readonly ICanUserAccessResourcesBusinessRule _canUserAccessResourcesBusinessRule;
    private readonly IDeletePermissionAssignmentsDbQuery _deletePermissionAssignmentsDbQuery;
    private readonly IPermissionAssignmentsComposer _permissionAssignmentsComposer;
    private readonly IPermissionAssignmentsLoader _permissionAssignmentsLoader;
    private readonly IAuthenticationTokenContext _authenticationTokenContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IUserLoader _userLoader;
    private readonly IIdentityDbContext _identityDbContext;


    public UpdateUserPermissionsRequest(IAuthenticatedUserContext authenticatedUserContext,
        IDeletePermissionAssignmentsDbQuery deletePermissionAssignmentsDbQuery,
        ICanUserAccessResourcesBusinessRule canUserAccessResourcesBusinessRule,
        IPermissionAssignmentsComposer permissionAssignmentsComposer,
        IPermissionAssignmentsLoader permissionAssignmentsLoader,
        IAuthenticationTokenContext authenticationTokenContext,
        IUserLoader userLoader,
        IIdentityDbContext identityDbContext)
    {
        _canUserAccessResourcesBusinessRule = canUserAccessResourcesBusinessRule;
        _permissionAssignmentsComposer = permissionAssignmentsComposer;
        _permissionAssignmentsLoader = permissionAssignmentsLoader;
        _authenticationTokenContext = authenticationTokenContext;
        _authenticatedUserContext = authenticatedUserContext;
        _userLoader = userLoader;
        _identityDbContext = identityDbContext;
        _deletePermissionAssignmentsDbQuery = deletePermissionAssignmentsDbQuery;
    }

    public async Task<List<PermissionAssignmentDto>> Handle(Command command,
        CancellationToken cancellationToken)
    {
        await CheckUserPermissionsCanBeUpdated(command, cancellationToken);

        var existingAssignmentIds = await _identityDbContext.PermissionAssignments
            .Where(pa =>
                    pa.UserId == command.UserId &&
                    pa.Permission!.Name == command.PermissionType.ToString() &&
                    pa.ResourceType == command.ResourceType.ToString())
            .Select(pa => pa.Id)
            .ToListAsync(cancellationToken);

        var existingAssignments = await _permissionAssignmentsLoader.LoadAsync(existingAssignmentIds.ToArray(),
            cancellationToken);

        var finalNewAssignments = new List<PermissionAssignmentCoreDto>(existingAssignments);
        var newAssignments = new List<PermissionAssignmentCoreDto>();

        var newResourceIds = command.Resources.Keys.Except(existingAssignments.Select(pa => pa.ResourceId)).ToList();
        if (newResourceIds.Count != 0)
        {
            var createdAssignments = await CreatePermissionsIfNecessaryAsync(command, newResourceIds,
                cancellationToken);

            newAssignments = createdAssignments.ToList();
            finalNewAssignments.AddRange(newAssignments);
        }

        var assignmentsToDelete = existingAssignments.ExceptBy(command.Resources.Keys, pa => pa.ResourceId).ToList();
        if (assignmentsToDelete.Count != 0)
        {
            var assignmentsToDeleteIds = assignmentsToDelete.Select(x => x.Id).ToArray();

            await _deletePermissionAssignmentsDbQuery.ExecuteAsync(assignmentsToDeleteIds, cancellationToken);

            finalNewAssignments = finalNewAssignments
                .Where(x => !assignmentsToDeleteIds.Contains(x.Id))
                .ToList();
        }

        await CancelSessionIfNecessaryAsync(command.UserId, assignmentsToDelete, newAssignments, cancellationToken);

        return finalNewAssignments.Select(x => PermissionAssignmentDto.Of(x)!).ToList();
    }

    private async Task<ICollection<PermissionAssignmentCoreDto>> CreatePermissionsIfNecessaryAsync(Command command,
        List<string?> newResourceIds,
        CancellationToken cancellationToken)
    {
        var newResources = command.Resources.Where(
                resource => newResourceIds.Any(id => id == resource.Key))
            .ToImmutableDictionary();

        return await _permissionAssignmentsComposer.ComposeAsync(
            new PermissionAssignmentsComposerRequest
            {
                UserId = command.UserId,
                ResourceType = command.ResourceType,
                Resources = newResources,
                PermissionType = command.PermissionType
            },
            cancellationToken);
    }

    private async Task CancelSessionIfNecessaryAsync(int userId, List<PermissionAssignmentCoreDto> assignmentsToDelete,
        List<PermissionAssignmentCoreDto> newAssignments,
        CancellationToken cancellationToken)
    {
        var hasDeletedAssignmentsWithTypes = assignmentsToDelete.Any(x => x is
        {
            PermissionType: PermissionType.TrackBillableWorkingHours,
            ResourceType: PermissionResourceType.Project
        });

        var hasCreatedAssignmentsWithTypes = newAssignments.Any(x => x is
        {
            PermissionType: PermissionType.TrackBillableWorkingHours,
            ResourceType: PermissionResourceType.Project
        });

        var shouldCancelUserSession = hasDeletedAssignmentsWithTypes || hasCreatedAssignmentsWithTypes;
        if (shouldCancelUserSession)
        {
            await _authenticationTokenContext.RevokeAllUserTokens(userId, RevokeReasons.PermissionAssignmentChanged,
                cancellationToken);
        }
    }

    private async Task CheckUserPermissionsCanBeUpdated(Command command, CancellationToken cancellationToken)
    {
        var user = await _userLoader.LoadAsync(command.UserId, cancellationToken);

        if (!user.RoleNames.IsEmployee())
        {
            throw new ActionNotAllowedException(PortalMessages.CannotGrantPermissionsToNonEmployeeUser);
        }

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment() &&
            _authenticatedUserContext.DepartmentId != user.DepartmentId)
        {
            throw new ActionNotAllowedException(PortalMessages
                .HeadOfDepartmentDoesNotHaveAccessToPermissionsOfUserFromAnotherDepartment);
        }

        if (user.DepartmentId is null)
        {
            throw new ActionNotAllowedException(PortalMessages.CannotGrantPermissionsToUserWithoutDepartment);
        }

        var ruleModel = new CanUserAccessResourcesBusinessRule.Request(command.UserId, command.ResourceType,
            command.Resources);

        var canUserAccessResources = await _canUserAccessResourcesBusinessRule
            .IsCorrespondsToBusiness(ruleModel, cancellationToken);

        if (!canUserAccessResources)
        {
            throw new ActionNotAllowedException(PortalMessages.UserDoesNotHaveAccessToSomeProjects);
        }
    }

    internal sealed record Command(
            int UserId,
            PermissionType PermissionType,
            IReadOnlyDictionary<string, string> Resources,
            PermissionResourceType ResourceType)
        : ICommand<List<PermissionAssignmentDto>>, ITransactionalRequest, IIdentityRequest;
}
