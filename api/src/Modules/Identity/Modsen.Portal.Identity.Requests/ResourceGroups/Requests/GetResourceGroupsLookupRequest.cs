using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.ResourceGroups.Requests.RequestDtos;

namespace Modsen.Portal.Identity.Requests.ResourceGroups.Requests;

internal sealed class GetResourceGroupsLookup : IQueryHandler<GetResourceGroupsLookup.Query, List<GroupLookupDto>>
{
    private readonly IIdentityDbContext _identityDbContext;

    public GetResourceGroupsLookup(IIdentityDbContext identityDbContext)
    {
        _identityDbContext = identityDbContext;
    }

    public async Task<List<GroupLookupDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var groups = _identityDbContext.ResourceGroups
            .AsNoTracking()
            .Where(resourceGroup => resourceGroup.IsActive)
            .Where(resourceGroup => request.DepartmentId == null || resourceGroup.DepartmentId == request.DepartmentId);

        if (request.HasGroupManager.HasValue)
        {
             var groupsWithManagersIds = await _identityDbContext.Users
                .Where(user => user.IsActive)
                .Where(user => user.UserRoles.First().Role!.Name == PortalRoleNames.GroupManager)
                .Where(user => user.ResourceGroupId != null)
                .Select(user => user.ResourceGroupId)
                .ToListAsync(cancellationToken);

            groups = request.HasGroupManager.Value
                ? groups.Where(x => groupsWithManagersIds.Contains(x.Id))
                : groups.Where(x => !groupsWithManagersIds.Contains(x.Id));
        }

        return await groups
            .OrderBy(x => x.Id)
            .Select(resourceGroup => new GroupLookupDto(
                resourceGroup.Id,
                resourceGroup.Name,
                resourceGroup.DepartmentId))
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query(string? DepartmentId, bool? HasGroupManager) : IQuery<List<GroupLookupDto>>;
}
