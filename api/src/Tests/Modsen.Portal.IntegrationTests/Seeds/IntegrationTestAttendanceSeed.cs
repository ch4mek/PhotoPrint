using Modsen.Portal.Attendance.DataAccess.Npgsql;
using Modsen.Portal.Attendance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Attendance;
using Modsen.Portal.Core.Database;

namespace Modsen.Portal.IntegrationTests.Seeds;

internal class IntegrationTestAttendanceSeed : ISeedEntitiesProvider<AttendanceDbContext>
{
    private readonly AttendanceDbContext _dbContext;

    public IntegrationTestAttendanceSeed(AttendanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Seed()
    {
        var dotnetG1 = _dbContext.ResourceGroups.FirstOrDefault(x => x.Name == ".NET Minsk - Group 1") ??
            throw new ArgumentNullException();

        var javaG1 = _dbContext.ResourceGroups.FirstOrDefault(x => x.Name == "Java Minsk - Group 1") ??
            throw new ArgumentNullException();

        var javaG2 = _dbContext.ResourceGroups.FirstOrDefault(x => x.Name == "Java Minsk - Group 2") ??
            throw new ArgumentNullException();

        var attendances = new[]
        {
            new AttendanceEntity
            {
                Id = 1,
                Door = "Door 1",
                Location = LocationType.Minsk,
                Direction = DirectionType.Enter,
                CheckedAtUtc = DateTime.UtcNow,
                User = new UserEntity
                {
                    UserId = 1_000_001,
                    FirstName = "Test1",
                    LastName = "Test1",
                    Email = "test1.test1@modsen-software.com",
                    FirstNameRu = "Тест1",
                    LastNameRu = "Тест1",
                    DepartmentId = null,
                    ResourceGroupId = null,
                    IsActive = true,
                }
            },
            new AttendanceEntity
            {
                Id = 2,
                Door = "Door 1",
                Location = LocationType.Minsk,
                Direction = DirectionType.Enter,
                CheckedAtUtc = DateTime.UtcNow,
                User = new UserEntity
                {
                    UserId = 1_000_002,
                    FirstName = "Test2",
                    LastName = "Test2",
                    Email = "test2.test2@modsen-software.com",
                    FirstNameRu = "Тест2",
                    LastNameRu = "Тест2",
                    DepartmentId = "Java",
                    ResourceGroupId = null,
                    IsActive = true,
                }
            },
            new AttendanceEntity
            {
                Id = 3,
                Door = "Door 1",
                Location = LocationType.Minsk,
                Direction = DirectionType.Enter,
                CheckedAtUtc = DateTime.UtcNow,
                User = new UserEntity
                {
                    UserId = 1_000_003,
                    FirstName = "Test3",
                    LastName = "Test3",
                    Email = "test3.test3@modsen-software.com",
                    FirstNameRu = "Тест3",
                    LastNameRu = "Тест3",
                    DepartmentId = "Net",
                    ResourceGroupId = dotnetG1.ResourceGroupId,
                    IsActive = true,
                }
            },
            new AttendanceEntity
            {
                Id = 4,
                Door = "Door 1",
                Location = LocationType.Minsk,
                Direction = DirectionType.Enter,
                CheckedAtUtc = DateTime.UtcNow,
                User = new UserEntity
                {
                    UserId = 1_000_004,
                    FirstName = "Test4",
                    LastName = "Test4",
                    Email = "test4.test4@modsen-software.com",
                    FirstNameRu = "Тест4",
                    LastNameRu = "Тест4",
                    DepartmentId = "Java",
                    ResourceGroupId = javaG1.ResourceGroupId,
                    IsActive = true,
                }
            },
            new AttendanceEntity
            {
                Id = 5,
                Door = "Door 1",
                Location = LocationType.Minsk,
                Direction = DirectionType.Enter,
                CheckedAtUtc = DateTime.UtcNow,
                User = new UserEntity
                {
                    UserId = 1_000_005,
                    FirstName = "Test5",
                    LastName = "Test5",
                    Email = "test5.test5@modsen-software.com",
                    FirstNameRu = "Тест5",
                    LastNameRu = "Тест5",
                    DepartmentId = "Java",
                    ResourceGroupId = javaG2.ResourceGroupId,
                    IsActive = true,
                }
            },
            new AttendanceEntity
            {
                Id = 6,
                Door = "Door 1",
                Location = LocationType.Minsk,
                Direction = DirectionType.Exit,
                CheckedAtUtc = DateTime.UtcNow.AddMinutes(200),
                UserId = 1_000_001,
            },
            new AttendanceEntity
            {
                Id = 7,
                Door = "Door 1",
                Location = LocationType.Minsk,
                Direction = DirectionType.Exit,
                CheckedAtUtc = DateTime.UtcNow.AddMinutes(300),                
                UserId = 1_000_002,
            },
            new AttendanceEntity
            {
                Id = 8,
                Door = "Door 1",
                Location = LocationType.Minsk,
                Direction = DirectionType.Exit,
                CheckedAtUtc = DateTime.UtcNow.AddMinutes(150),
                UserId = 1_000_003,
            },
            new AttendanceEntity
            {
                Id = 9,
                Door = "Door 1",
                Location = LocationType.Minsk,
                Direction = DirectionType.Exit,
                CheckedAtUtc = DateTime.UtcNow.AddMinutes(350),
                UserId = 1_000_004,
            },
            new AttendanceEntity
            {
                Id = 10,
                Door = "Door 1",
                Location = LocationType.Minsk,
                Direction = DirectionType.Exit,
                CheckedAtUtc = DateTime.UtcNow.AddMinutes(120),
                UserId = 1_000_005,
            },
        };

        _dbContext.Attendances.AddRange(attendances);
        _dbContext.SaveChanges();
    }
}
