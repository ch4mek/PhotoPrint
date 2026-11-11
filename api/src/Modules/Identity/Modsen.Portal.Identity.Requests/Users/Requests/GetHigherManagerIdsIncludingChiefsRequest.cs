using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Core;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class GetHigherManagerIdsIncludingChiefsRequest : IQueryHandler<GetHigherManagerIdsIncludingChiefsRequest.Query, int[]>
{
    private readonly IUserLoader _userLoader;
    private readonly IIdentityDbContext _identityDbContext;

    public GetHigherManagerIdsIncludingChiefsRequest(
        IIdentityDbContext identityDbContext,
        IUserLoader userLoader)
    {
        _identityDbContext = identityDbContext;
        _userLoader = userLoader;
    }

    public async Task<int[]> Handle(Query request, CancellationToken cancellationToken)
    {
        var user = await _userLoader.LoadAsync(request.UserId, cancellationToken);

        switch (user.Position)
        {
            case PortalRoleNames.Employee:
            case PortalRoleNames.SystemAdministrator:
                {
                    List<int> managersIds = [];

                    if (user.ResourceGroupId != null)
                    {
                        var groupManagerId = await _identityDbContext.Users
                            .Where(u => u.IsActive)
                            .Where(u => u.UserRoles.Any(ur => ur.Role!.Name == PortalRoleNames.GroupManager))
                            .Where(u => u.ResourceGroupId == user.ResourceGroupId.Value)
                            .Select(u => u.Id)
                            .FirstOrDefaultAsync(cancellationToken);

                        if (groupManagerId != 0)
                        {
                            managersIds.Add(groupManagerId);
                        }
                    }

                    var headOfDepartmentId = await GetHeadOfDepartmentIdByDepartmentId(user.DepartmentId, cancellationToken);
                    if (headOfDepartmentId != 0)
                    {
                        managersIds.Add(headOfDepartmentId);
                    }

                    var chiefId = await GetChiefIdByDepartmentId(user.DepartmentId, cancellationToken);
                    managersIds.Add(chiefId);

                    return managersIds.ToArray();
                }
            case PortalRoleNames.GroupManager:
                {
                    List<int> managerIds = [];

                    var headOfDepartmentId = await GetHeadOfDepartmentIdByDepartmentId(user.DepartmentId, cancellationToken);
                    if (headOfDepartmentId != 0)
                    {
                        managerIds.Add(headOfDepartmentId);
                    }

                    var chiefId = await GetChiefIdByDepartmentId(user.DepartmentId, cancellationToken);
                    managerIds.Add(chiefId);

                    return managerIds.ToArray();
                }
            case PortalRoleNames.HeadOfDepartment:
                {
                    var chiefId = await GetChiefIdByDepartmentId(user.DepartmentId, cancellationToken);

                    return [chiefId];
                }
            case PortalRoleNames.Cto:
                {
                    var ceoId = await GetUserIdsByRoleName(PortalRoleNames.Ceo, cancellationToken);

                    return [ceoId.FirstOrDefault()];
                }
            case PortalRoleNames.Ceo:
                {
                    return [user.Id];
                }
            default:
                {
                    return [];
                }
        }
    }

    internal sealed record Query(int UserId) : IQuery<int[]>, IIdentityRequest;

    private Task<int> GetHeadOfDepartmentIdByDepartmentId(string? departmentId, CancellationToken cancellationToken)
    {
        return _identityDbContext.Users
            .Where(x => x.IsActive && x.UserRoles.Any(role => role.Role!.Name == PortalRoleNames.HeadOfDepartment))
            .Where(x => x.DepartmentId == departmentId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private Task<List<int>> GetUserIdsByRoleName(string roleName, CancellationToken cancellationToken)
    {
        return _identityDbContext.Users
            .Where(x => x.IsActive)
            .Where(x => x.UserRoles.Any(role => role.Role!.Name == roleName))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    private async Task<int> GetChiefIdByDepartmentId(string? departmentId, CancellationToken cancellationToken)
    {
        var reportingRoleId = await _identityDbContext.Departments
            .Where(x => x.Id == departmentId)
            .Select(x => x.DepartmentReportingToRoleId)
            .FirstAsync(cancellationToken);

        return await _identityDbContext.Users
            .Where(x => x.IsActive)
            .Where(x => x.UserRoles.Any(ur => ur.RoleId == reportingRoleId))
            .Select(x => x.Id)
            .FirstAsync(cancellationToken);
    }
}
