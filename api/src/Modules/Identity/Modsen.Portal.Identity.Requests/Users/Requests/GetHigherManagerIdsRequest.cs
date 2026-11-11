using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Core;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class GetHigherManagerIdsRequest : IQueryHandler<GetHigherManagerIdsRequest.Query, int[]>
{
    private readonly IUserLoader _userLoader;
    private readonly IIdentityDbContext _identityDbContext;

    public GetHigherManagerIdsRequest(
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
                    var headOfDepartmentId = await GetHeadOfDepartmentIdByDepartmentId(user.DepartmentId, cancellationToken);

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
                            return [groupManagerId, headOfDepartmentId];
                        }
                    }

                    return [headOfDepartmentId];
                }
            case PortalRoleNames.GroupManager:
                {
                    var headOfDepartmentId = await GetHeadOfDepartmentIdByDepartmentId(user.DepartmentId, cancellationToken);

                    return [headOfDepartmentId];
                }
            case PortalRoleNames.HeadOfDepartment:
                {
                    var ctoId = await GetUserIdsByRoleName(PortalRoleNames.Cto, cancellationToken);

                    return [ctoId.FirstOrDefault()];
                }
            case PortalRoleNames.Cto:
                {
                    var ceoId = await GetUserIdsByRoleName(PortalRoleNames.Ceo, cancellationToken);

                    return [ceoId.FirstOrDefault()];
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
}
