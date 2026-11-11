using System.Globalization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Requests.RequestDto;
using Modsen.Portal.TimeTrack.Contract;

namespace Modsen.Portal.Identity.Requests.PermissionAssignments.Requests;

internal sealed class GetUserPermissionResourcesRequest
    : IQueryHandler<GetUserPermissionResourcesRequest.Query, IReadOnlyCollection<PermissionResource>>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IUserLoader _userLoader;
    private readonly IIdentityDbContext _identityDbContext;
    private readonly ITimeTrackContract _timeTrackContract;

    public GetUserPermissionResourcesRequest(IAuthenticatedUserContext authenticatedUserContext,
        IUserLoader userLoader,
        ITimeTrackContract timeTrackContract,
        IIdentityDbContext identityDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _timeTrackContract = timeTrackContract;
        _userLoader = userLoader;
        _identityDbContext = identityDbContext;
    }

    public async Task<IReadOnlyCollection<PermissionResource>> Handle(Query request,
        CancellationToken cancellationToken)
    {
        await CheckCurrentUserHasAccessToAnotherUser(request.UserId, cancellationToken);

        return request.ResourceType switch
        {
            PermissionResourceType.Project => await GetProjectResources(request.UserId, request.PermissionType, cancellationToken),
            _ => throw new InvalidOperationException(PortalMessages.UnsupportedResourceType),
        };
    }

    private async Task<IReadOnlyCollection<PermissionResource>> GetProjectResources(int userId,
        PermissionType permissionType,
        CancellationToken cancellationToken)
    {
        var projectIds = await _identityDbContext.PermissionAssignments
            .AsNoTracking()
            .Where(pa =>
                pa.UserId == userId &&
                pa.Permission!.Name == permissionType.ToString())
            .Select(pa => int.Parse(pa.ResourceId!, CultureInfo.InvariantCulture))
            .ToListAsync(cancellationToken);

        var projects = await _timeTrackContract
            .GetListOfProjectsByIds(projectIds, cancellationToken);

        return projects.Select(projectOutput => new PermissionResource
        {
            ResourceId = projectOutput.Id.ToString(CultureInfo.InvariantCulture),
            ResourceName = projectOutput.Name
        }).ToList();
    }

    private async Task CheckCurrentUserHasAccessToAnotherUser(int userId, CancellationToken cancellationToken)
    {
        var user = await _userLoader.LoadAsync(userId, cancellationToken);

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment()
            && _authenticatedUserContext.DepartmentId != user.DepartmentId)
        {
            throw new ActionNotAllowedException(PortalMessages
                .HeadOfDepartmentDoesNotHaveAccessToPermissionsOfUserFromAnotherDepartment);
        }
    }

    internal sealed record Query(
            int UserId,
            PermissionType PermissionType,
            PermissionResourceType ResourceType)
        : IQuery<IReadOnlyCollection<PermissionResource>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.ResourceType).IsInEnum();
        }
    }
}
