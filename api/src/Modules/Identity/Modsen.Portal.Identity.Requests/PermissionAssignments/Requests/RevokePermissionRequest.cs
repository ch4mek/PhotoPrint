using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.CoreDto;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.DbQueries;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.Loaders;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Requests.RequestDto;

namespace Modsen.Portal.Identity.Requests.PermissionAssignments.Requests;

internal sealed class RevokePermissionRequest
    : ICommandHandler<RevokePermissionRequest.Command, PermissionAssignmentDto>
{
    private readonly IDeletePermissionAssignmentsDbQuery _deletePermissionAssignmentsDbQuery;
    private readonly IPermissionAssignmentsLoader _permissionAssignmentsLoader;
    private readonly IAuthenticationTokenContext _authenticationTokenContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IUserLoader _userLoader;

    public RevokePermissionRequest(IDeletePermissionAssignmentsDbQuery deletePermissionAssignmentsDbQuery,
        IPermissionAssignmentsLoader permissionAssignmentsLoader,
        IAuthenticationTokenContext authenticationTokenContext,
        IAuthenticatedUserContext authenticatedUserContext,
        IUserLoader userLoader)
    {
        _deletePermissionAssignmentsDbQuery = deletePermissionAssignmentsDbQuery;
        _permissionAssignmentsLoader = permissionAssignmentsLoader;
        _authenticationTokenContext = authenticationTokenContext;
        _authenticatedUserContext = authenticatedUserContext;
        _userLoader = userLoader;
    }

    public async Task<PermissionAssignmentDto> Handle(Command command, CancellationToken cancellationToken)
    {
        var permissionAssignmentId = command.PermissionAssignmentId;

        var permissionAssignment = await _permissionAssignmentsLoader.LoadAsync(permissionAssignmentId,
            cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.PermissionAssignmentNotFound);

        await CheckUserPermissionCanBeRevoked(permissionAssignment, cancellationToken);

        await _deletePermissionAssignmentsDbQuery.ExecuteAsync([permissionAssignmentId], cancellationToken);

        if (permissionAssignment is
            {
                PermissionType: PermissionType.TrackBillableWorkingHours,
                ResourceType: PermissionResourceType.Project
            })
        {
            await _authenticationTokenContext.RevokeAllUserTokens(permissionAssignment.UserId,
                reason: RevokeReasons.ProjectPermissionsChanged, cancellationToken);
        }

        return PermissionAssignmentDto.Of(permissionAssignment)!;
    }

    private async Task CheckUserPermissionCanBeRevoked(
        PermissionAssignmentCoreDto permissionAssignmentCoreDto,
        CancellationToken cancellationToken)
    {
        var user = await _userLoader.LoadAsync(permissionAssignmentCoreDto.UserId, cancellationToken);

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment()
            && _authenticatedUserContext.DepartmentId != user.DepartmentId)
        {
            throw new ActionNotAllowedException(PortalMessages
                .HeadOfDepartmentDoesNotHaveAccessToPermissionsOfUserFromAnotherDepartment);
        }
    }

    internal sealed record Command(int PermissionAssignmentId)
        : ICommand<PermissionAssignmentDto>, ITransactionalRequest, IIdentityRequest;
}
