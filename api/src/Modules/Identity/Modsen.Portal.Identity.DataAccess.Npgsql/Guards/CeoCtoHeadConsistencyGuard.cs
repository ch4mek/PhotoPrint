using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.Guards;

internal class CeoCtoHeadConsistencyGuard : IDataConsistencyGuard
{
    private readonly IdentityModuleDbContext _identityModuleDbContext;

    public CeoCtoHeadConsistencyGuard(IdentityModuleDbContext identityModuleDbContext)
    {
        _identityModuleDbContext = identityModuleDbContext;
    }

    public void Verify()
    {
        EnsureSingleActiveUser(PortalRoleNames.Ceo);
        EnsureSingleActiveUser(PortalRoleNames.Cto);

        foreach (var departmentId in GetDepartmentIds())
        {
            EnsureSingleActiveUser(PortalRoleNames.HeadOfDepartment, departmentId);
        }
    }

    private List<string> GetDepartmentIds()
    {
        return _identityModuleDbContext.Set<Department>().Select(department => department.Id).ToList();
    }

    private void EnsureSingleActiveUser(string roleName, string? departmentId = null)
    {
        if (GetCountOfUsersWithRole(roleName, departmentId) > 1)
        {
            throw new AppException($"There are multiple {roleName} users within the system. Department: {departmentId}");
        }
    }

    private int GetCountOfUsersWithRole(string roleName, string? departmentId = null)
    {
        var query = _identityModuleDbContext.Users.Where(user => !user.IsActive);

        if (departmentId is not null)
        {
            query = query.Where(user => user.DepartmentId == departmentId);
        }

        return query.Count(user => user.UserRoles.Any(userRole => userRole.Role!.Name == roleName));
    }
}
