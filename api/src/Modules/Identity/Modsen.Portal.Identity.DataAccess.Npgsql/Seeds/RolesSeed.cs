using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.Seeds;

internal class RolesSeed : ISeedEntitiesProvider<IdentityModuleDbContext>
{
    private readonly IdentityModuleDbContext _dbContext;
    private readonly ISeedClaimProvider<IdentityModuleDbContext, Role>[] _claimsSeedProviders;

    public RolesSeed(
        IdentityModuleDbContext dbContext,
        IEnumerable<ISeedClaimProvider<IdentityModuleDbContext, Role>> claimsSeedProviders)
    {
        _dbContext = dbContext;
        _claimsSeedProviders = claimsSeedProviders
            .Where(x => x.Scope == nameof(RoleClaim))
            .ToArray();
    }

    public void Seed()
    {
        AddRole(PortalRoleNames.Ceo, 1000, "CEO", true);
        AddRole(PortalRoleNames.Cto, 900, "CTO", true);
        AddRole(PortalRoleNames.HeadOfDepartment, 700, "Head of Department", true);
        AddRole(PortalRoleNames.GroupManager, 400, "Group Manager", true);
        AddRole(PortalRoleNames.Employee, 100, "Employee");
        AddRole(PortalRoleNames.SystemAdministrator, 99, "System Administrator");
        AddRole(PortalRoleNames.Intern, 98, "Intern");

        _dbContext.SaveChanges();

        var roles = _dbContext.Roles.ToArray();

        foreach (var role in roles)
        {
            foreach (var claimsSeedProvider in _claimsSeedProviders)
            {
                claimsSeedProvider.Seed(role);
                _dbContext.SaveChanges();
            }
        }
    }

    private void AddRole(string name, int orderWeight, string publicName, bool isCompanyManager = false)
    {
        if (!_dbContext.Roles.Any(x => x.NormalizedName == name.ToUpperInvariant()))
        {
            var role = new Role
            {
                Name = name,
                NormalizedName = name.ToUpperInvariant(),
                OrderWeight = orderWeight,
                PublicName = publicName,
                IsCompanyManager = isCompanyManager
            };

            _dbContext.Roles.Add(role);
        }
    }
}
