using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.Seeds;

internal class PermissionsSeed : ISeedEntitiesProvider<IdentityModuleDbContext>
{
    private readonly IdentityModuleDbContext _dbContext;

    public PermissionsSeed(IdentityModuleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Seed()
    {
        var permissions = _dbContext.PermissionDefinitions;

        AddPermissionIfNotExists(permissions, PortalPermissionNames.TrackBillableWorkingHours);
        AddPermissionIfNotExists(permissions, PortalPermissionNames.BookWorkplace);
        AddPermissionIfNotExists(permissions, PortalPermissionNames.BookMeetingRoom);

        _dbContext.SaveChanges();
    }

    private static void AddPermissionIfNotExists(
        DbSet<PermissionDefinition> permissionDefinitions,
        string permissionName,
        string? description = null)
    {
        var hasAny = permissionDefinitions.Any(x => x.Name == permissionName);

        if (!hasAny)
        {
            permissionDefinitions.Add(new PermissionDefinition
            {
                Name = permissionName,
                Description = description
            });
        }
    }
}
