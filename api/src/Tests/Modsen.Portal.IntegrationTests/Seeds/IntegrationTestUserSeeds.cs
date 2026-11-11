using Modsen.Portal.Core;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.SettingNames;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.IntegrationTests.Seeds;

internal class IntegrationTestUserSeeds : ISeedEntitiesProvider<IdentityModuleDbContext>
{
    private readonly IdentityModuleDbContext _dbContext;
    private readonly IClockService _clock;
    private readonly IHexColorGenerator _hexColorGenerator;

    public IntegrationTestUserSeeds(
        IdentityModuleDbContext dbContext,
        IClockService clock,
        IHexColorGenerator hexColorGenerator)
    {
        _dbContext = dbContext;
        _clock = clock;
        _hexColorGenerator = hexColorGenerator;
    }

    public void Seed()
    {
        CreateUser("snapshot.test@modsen-software.com", "Snapshot", "Test", LocationType.Minsk);
        CreateUser("groupmanager.java.test@modsen-software.com", "GroupManager", "Test",
            LocationType.Minsk, role: PortalRoleNames.GroupManager, departmentId: "Java");
        CreateUser("head.of.qa@modsen-software.com", "Head", "Pmo",
            LocationType.Minsk, role: PortalRoleNames.HeadOfDepartment, departmentId: "QA");
        CreateUser(EmailConstants.SysAdminHead, "Head", "SysAdmin",
            LocationType.Minsk, role: PortalRoleNames.HeadOfDepartment, departmentId: "SysAdmin");
    }

    private void CreateUser(string email, string firstName, string lastName,
        LocationType location = LocationType.Vitebsk, string role = PortalRoleNames.Employee,
        string? departmentId = "Java", bool isActive = true, int registeredLocationId = PortalSharedConstants.DefaultRegisteredLocationId)
    {
        var supportUser = _dbContext.Users.First(x => x.Email == "support@modsen-software.com");
        if (!_dbContext.Users.Any(x => x.Email == email))
        {
            var timezone = _dbContext.PortalSettings.FirstOrDefault(x => x.Name == PortalSettingNames.DefaultTimeZone)?.Value ?? "BY";

            var user = new User
            {
                Email = email,
                NormalizedEmail = email.UpperInvariantNormalize(),
                UserName = email,
                NormalizedUserName = email.UpperInvariantNormalize(),
                FirstName = firstName,
                LastName = lastName,
                DepartmentId = departmentId,
                EmailConfirmed = true,
                CreatedAtUtc = _clock.UtcNow,
                CreatedById = supportUser.Id,
                SecurityStamp = Guid.CreateVersion7().ToString(PortalSharedConstants.DefaultGuidFormat),
                Location = location,
                RegisteredLocationId = registeredLocationId,
                Color = _hexColorGenerator.GenerateColor(),
                TimeZone = timezone,
                IsActive = isActive
            };

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            var roleId = _dbContext.Roles.First(x => x.Name == role).Id;
            _dbContext.UserRoles.Add(new UserRole { RoleId = roleId, UserId = user.Id });
            _dbContext.SaveChanges();
        }
    }
}
