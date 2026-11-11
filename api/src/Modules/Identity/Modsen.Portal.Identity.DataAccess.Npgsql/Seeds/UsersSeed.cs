using Bogus;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.SettingNames;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.Seeds;

internal class UsersSeed : ISeedEntitiesProvider<IdentityModuleDbContext>
{
    private readonly IdentityModuleDbContext _dbContext;
    private readonly IClockService _clock;
    private readonly IHexColorGenerator _hexColorGenerator;
    private readonly IHostEnvironmentContext _hostEnvironmentContext;
    private readonly ISeedClaimProvider<IdentityModuleDbContext, User>[] _claimsSeedProviders;

    public UsersSeed(
        IdentityModuleDbContext dbContext,
        IClockService clock,
        IHexColorGenerator hexColorGenerator,
        IHostEnvironmentContext hostEnvironmentContext,
        IEnumerable<ISeedClaimProvider<IdentityModuleDbContext, User>> claimsSeedProviders)
    {
        _dbContext = dbContext;
        _clock = clock;
        _hexColorGenerator = hexColorGenerator;
        _hostEnvironmentContext = hostEnvironmentContext;
        _claimsSeedProviders = claimsSeedProviders
            .Where(x => x.Scope == nameof(UserClaim))
            .ToArray();
    }

    private int CountOfFakeUsers =>
        _hostEnvironmentContext.IsFunctionalTestEnvironment() ||
        _hostEnvironmentContext.IsIntegrationTestEnvironment() ? 1 : 1000;

    public void Seed()
    {
        if (_hostEnvironmentContext.IsProductionEnvironment())
        {
            return;
        }

        var timezone = _dbContext.PortalSettings.FirstOrDefault(x => x.Name == PortalSettingNames.DefaultTimeZone)?.Value ?? "BY";

        if (_hostEnvironmentContext.IsFunctionalTestEnvironment() ||
            _hostEnvironmentContext.IsIntegrationTestEnvironment() ||
            _hostEnvironmentContext.IsDevelopmentEnvironment() ||
            _hostEnvironmentContext.IsQaEnvironment() ||
            _hostEnvironmentContext.IsStagingEnvironment())
        {
            var employeeRoleId = _dbContext.Roles.First(x => x.Name == PortalRoleNames.Employee).Id;

            CreateUser("support@modsen-software.com", "System", "User", timezone);
            CreateUser("modsen.portal.test.ceo@modsen-software.com", "Test", "CEO", timezone, LocationType.Minsk, PortalRoleNames.Ceo);
            CreateUser("modsen.portal.test.cto@modsen-software.com", "Test", "CTO", timezone, LocationType.Minsk, PortalRoleNames.Cto);
            CreateUser("modsen.portal.test.headofdepartment@modsen-software.com", "Test", "HeadOfDepartment", timezone, LocationType.Minsk, PortalRoleNames.HeadOfDepartment, departmentId: "Net");
            CreateUser("modsen.portal.test.sysadmin@modsen-software.com", "Test", "SysAdmin", timezone, LocationType.Minsk, PortalRoleNames.SystemAdministrator, departmentId: "Administration");
            CreateUser("modsen.portal.test.employee@modsen-software.com", "Test", "Employee", timezone, LocationType.Minsk, roleId: employeeRoleId, departmentId: "Net");
            CreateUser("modsen.portal.test.groupmanager@modsen-software.com", "Test", "GroupManager", timezone, LocationType.Minsk, PortalRoleNames.GroupManager, departmentId: "Net");
            CreateUser("modsen.portal.test.intern@modsen.by", "Test", "Intern", timezone, LocationType.Minsk, PortalRoleNames.Intern, departmentId: "LD");

            // Users approving inquiry requests
            // if they have changed, the UserClaimSeed should also be changed
            CreateUser("yuliya.lapo@modsen-software.com", "Yuliya", "Lapo", timezone, LocationType.Minsk, roleId: employeeRoleId);
            CreateUser("volha.buber@modsen-software.com", "Volha", "Buber", timezone, LocationType.Minsk, roleId: employeeRoleId);
            CreateUser("hanna.brit@modsen-software.com", "Hanna", "Brit", timezone, LocationType.Minsk, roleId: employeeRoleId);
            CreateUser("tatiana.gvardeitseva@modsen-software.com", "Tatiana", "Gvardeitseva", timezone, LocationType.Minsk, roleId: employeeRoleId);

            CreateUser("aliaksandr.yakavets@modsen-software.com", "Aliaksandr", "Yakavets", timezone, LocationType.Minsk, PortalRoleNames.HeadOfDepartment, departmentId: "PMO");

            // Add fake users
            if (_dbContext.Users.Count() < CountOfFakeUsers)
            {
                var faker = new Faker();
                var departmentIds = _dbContext.Set<Department>().Select(x => x.Id).ToList();

                for (var i = 0; i < CountOfFakeUsers; i++)
                {
                    var firstName = faker.Name.FirstName();
                    var lastName = faker.Name.LastName();

                    var departmentId = faker.Random.ListItem(departmentIds);
                    var location = faker.Random.Enum((LocationType)0);
                    var isActive = faker.Random.Bool(0.8f);

                    CreateUser($"{firstName}.{lastName}@modsen-software.com", firstName, lastName, timezone, location,
                        roleId: employeeRoleId, departmentId: departmentId, isActive: isActive);
                }
            }
        }

        if (_hostEnvironmentContext.IsQaEnvironment() ||
            _hostEnvironmentContext.IsStagingEnvironment())
        {
            CreateUser("gleb.terentev@modsen-software.com", "Gleb", "Terentev", timezone,  LocationType.Vitebsk, PortalRoleNames.Ceo);
            CreateUser("eugene.kalugin@modsen-software.com", "Eugene", "Kalugin", timezone, LocationType.Minsk, PortalRoleNames.Cto);
        }

        SeedUsersForAttendanceModule(timezone);
        SeedUsersWithChangedDepartment(timezone);

        var users = _dbContext.Users
            .Include(x=>x.UserRoles)
            .ThenInclude(x=>x.Role)
            .ToArray();

        foreach (var user in users)
        {
            foreach (var claimsSeedProvider in _claimsSeedProviders)
            {
                claimsSeedProvider.Seed(user);
            }
        }

        _dbContext.SaveChanges();
    }

    private void CreateUser(string email, string firstName, string lastName, string timezone,
        LocationType location = LocationType.Vitebsk, string role = PortalRoleNames.Employee, int? roleId = null,
        string? departmentId = null, bool isActive = true, int countryId = PortalSharedConstants.DefaultCountryId,
        int registeredLocationId = PortalSharedConstants.DefaultRegisteredLocationId)
    {
        if (!_dbContext.Users.Any(x => x.Email == email))
        {
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
                CreatedById = null,
                SecurityStamp = Guid.CreateVersion7().ToString(PortalSharedConstants.DefaultGuidFormat),
                Location = location,
                Color = _hexColorGenerator.GenerateColor(),
                TimeZone = timezone,
                IsActive = isActive,
                CountryId = countryId,
                RegisteredLocationId = registeredLocationId
            };

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            _dbContext.UserRoles.Add(new UserRole
            {
                RoleId = roleId ?? _dbContext.Roles.First(x => x.Name == role).Id,
                UserId = user.Id
            });
            _dbContext.SaveChanges();
        }
    }

    private void SeedUsersForAttendanceModule(string timezone)
    {
        if (!(_hostEnvironmentContext.IsDevelopmentEnvironment() ||
                _hostEnvironmentContext.IsQaEnvironment() ||
                _hostEnvironmentContext.IsStagingEnvironment()))
        {
            return;
        }

        CreateUser("attendance.test1@modsen-software.com", "Attendance", "Test1", timezone, departmentId: "Net");
        CreateUser("attendance.test2@modsen-software.com", "Attendance", "Test2", timezone, departmentId: "Net");
        CreateUser("attendance.test3@modsen-software.com", "Attendance", "Test3", timezone, departmentId: "Net");
        CreateUser("attendance.test4@modsen-software.com", "Attendance", "Test4", timezone, departmentId: null);
    }

    private void SeedUsersWithChangedDepartment(string timezone)
    {
        if (!(_hostEnvironmentContext.IsDevelopmentEnvironment() ||
                _hostEnvironmentContext.IsQaEnvironment() ||
                _hostEnvironmentContext.IsStagingEnvironment() ||
                _hostEnvironmentContext.IsFunctionalTestEnvironment() ||
                _hostEnvironmentContext.IsIntegrationTestEnvironment()))
        {
            return;
        }

        CreateUser("dep.change.test1@modsen-software.com", "DepChange", "Test1", timezone, departmentId: "Java");
        CreateUser("dep.change.test2@modsen-software.com", "DepChange", "Test2", timezone, departmentId: "Java");
        CreateUser("dep.change.test3@modsen-software.com", "DepChange", "Test3", timezone, departmentId: "Java");
        CreateUser("dep.change.test4@modsen-software.com", "DepChange", "Test4", timezone, departmentId: "Java");
        CreateUser("dep.change.test5@modsen-software.com", "DepChange", "Test5", timezone, departmentId: "Java");
        CreateUser("dep.change.test6@modsen-software.com", "DepChange", "Test6", timezone, departmentId: "Java");
        CreateUser("dep.change.test7@modsen-software.com", "DepChange", "Test7", timezone, departmentId: "Java");
        CreateUser("dep.change.test8@modsen-software.com", "DepChange", "Test8", timezone, departmentId: "Java");
        CreateUser("dep.change.test9@modsen-software.com", "DepChange", "Test9", timezone, departmentId: "Java");
        CreateUser("dep.change.test10@modsen-software.com", "DepChange", "Test10", timezone, departmentId: "Java");
    }
}
