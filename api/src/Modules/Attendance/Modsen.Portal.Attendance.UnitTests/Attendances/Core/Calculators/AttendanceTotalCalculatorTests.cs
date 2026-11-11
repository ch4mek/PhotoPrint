using Modsen.Portal.Attendance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Attendance.DataAccess.Npgsql.OData;
using Modsen.Portal.Attendance.Requests.Attendances.Core.Calculators;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Attendance;
using Modsen.Portal.Core.Utils;

namespace Modsen.Portal.Attendance.UnitTests.Attendances.Core.Calculators;

public class AttendanceTotalCalculatorTests
{
    private readonly AttendanceTotalCalculator _calculator = new();

    [Fact]
    public void CalculateWhenTwoSequentialEntersShouldReturnValid()
    {
        // Arrange
        var duration = 2 * 60;
        var enter = DateTime.UtcNow.Date.AddHours(10);
        var exit = enter.AddMinutes(duration);

        var attendances = new List<AttendanceEntity>
        {
            new AttendanceEntity
            {
                Id = 1,
                UserId = 1,
                User = new UserEntity
                {
                    FirstName = "First",
                    LastName = "Last",
                    Email = "email",
                    DepartmentId = "Net",
                },
                CheckedAtUtc = enter.AddHours(-3),
                Door = "Door",
                Direction = DirectionType.Enter,
                Location = LocationType.Warsaw
            },
            new AttendanceEntity
            {
                Id = 2,
                UserId = 1,
                User = new UserEntity
                {
                    FirstName = "First",
                    LastName = "Last",
                    Email = "email",
                    DepartmentId = "Net",
                },
                CheckedAtUtc = enter,
                Door = "Door",
                Direction = DirectionType.Enter,
                Location = LocationType.Warsaw
            },
            new AttendanceEntity
            {
                Id = 3,
                UserId = 1,
                User = new UserEntity
                {
                    FirstName = "First",
                    LastName = "Last",
                    Email = "email",
                    DepartmentId = "Net",
                },
                CheckedAtUtc = exit,
                Door = "Door",
                Direction = DirectionType.Exit,
                Location = LocationType.Warsaw
            },
        };

        var expected = new List<OdpAttendanceTotal>()
        {
            new OdpAttendanceTotal
            {
                UserId = attendances[0].UserId,
                UserDepartmentId = attendances[0].User!.DepartmentId,
                UserFirstName = attendances[0].User!.FirstName,
                UserLastName = attendances[0].User!.LastName,
                UserFullName = ToFullName.Reverse(attendances[0].User!.FirstName, attendances[0].User!.LastName),
                Date = DateOnly.FromDateTime(enter.Date),
                TotalTimeInMinutes = duration
            }
        };

        // Act
        var result = _calculator.Calculate(attendances);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void CalculateWhenTwoSequentialExitsShouldReturnValid()
    {
        // Arrange
        var duration = 2 * 60;
        var enter = DateTime.UtcNow.Date.AddHours(10);
        var exit = enter.AddMinutes(duration);

        var attendances = new List<AttendanceEntity>
        {
            new AttendanceEntity
            {
                Id = 1,
                UserId = 1,
                User = new UserEntity
                {
                    FirstName = "First",
                    LastName = "Last",
                    Email = "email",
                    DepartmentId = "Net",
                },
                CheckedAtUtc = enter,
                Door = "Door",
                Direction = DirectionType.Enter,
                Location = LocationType.Warsaw
            },
            new AttendanceEntity
            {
                Id = 2,
                UserId = 1,
                User = new UserEntity
                {
                    FirstName = "First",
                    LastName = "Last",
                    Email = "email",
                    DepartmentId = "Net",
                },
                CheckedAtUtc = exit,
                Door = "Door",
                Direction = DirectionType.Exit,
                Location = LocationType.Warsaw
            },
            new AttendanceEntity
            {
                Id = 3,
                UserId = 1,
                User = new UserEntity
                {
                    FirstName = "First",
                    LastName = "Last",
                    Email = "email",
                    DepartmentId = "Net",
                },
                CheckedAtUtc = exit.AddHours(3),
                Door = "Door",
                Direction = DirectionType.Exit,
                Location = LocationType.Warsaw
            },
        };

        var expected = new List<OdpAttendanceTotal>()
        {
            new OdpAttendanceTotal
            {
                UserId = attendances[0].UserId,
                UserDepartmentId = attendances[0].User!.DepartmentId,
                UserFirstName = attendances[0].User!.FirstName,
                UserLastName = attendances[0].User!.LastName,
                UserFullName = ToFullName.Reverse(attendances[0].User!.FirstName, attendances[0].User!.LastName),
                Date = DateOnly.FromDateTime(enter.Date),
                TotalTimeInMinutes = duration
            }
        };

        // Act
        var result = _calculator.Calculate(attendances);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void CalculateWhenTimeBetweenEnterAndExitGreaterThanLimitShouldReturnValid()
    {
        // Arrange
        var enter = DateTime.UtcNow;
        var exit = DateTime.UtcNow.AddHours(PortalSharedConstants.AttendanceMaxHours + 2);

        var attendances = new List<AttendanceEntity>
        {
            new AttendanceEntity
            {
                Id = 1,
                UserId = 1,
                User = new UserEntity
                {
                    FirstName = "First",
                    LastName = "Last",
                    Email = "email",
                    DepartmentId = "Net",
                },
                CheckedAtUtc = enter,
                Door = "Door",
                Direction = DirectionType.Enter,
                Location = LocationType.Warsaw
            },
            new AttendanceEntity
            {
                Id = 2,
                UserId = 1,
                User = new UserEntity
                {
                    FirstName = "First",
                    LastName = "Last",
                    Email = "email",
                    DepartmentId = "Net",
                },
                CheckedAtUtc = exit,
                Door = "Door",
                Direction = DirectionType.Exit,
                Location = LocationType.Warsaw
            }
        };

        // Act
        var result = _calculator.Calculate(attendances);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public void CalculateWhenDoubleDayAttendanceShouldReturnValid()
    {
        // Arrange
        var durationToMidnightUtc = 4 * 60;
        var durationFromMidnightUtc = 3 * 60;
        var enterUtc = DateTime.UtcNow.Date.AddDays(1).AddMinutes(-durationToMidnightUtc);
        var exitUtc = DateTime.UtcNow.Date.AddDays(1).AddMinutes(durationFromMidnightUtc);
        var durationToMidnightVitebskLocal = 60;
        var durationFromMidnightVitebskLocal = 6 * 60;

        var attendances = new List<AttendanceEntity>
        {
            new AttendanceEntity
            {
                Id = 1,
                UserId = 1,
                User = new UserEntity
                {
                    FirstName = "First",
                    LastName = "Last",
                    Email = "email",
                    DepartmentId = "Net",
                },
                CheckedAtUtc = enterUtc,
                Door = "Door",
                Direction = DirectionType.Enter,
                Location = LocationType.Vitebsk
            },
            new AttendanceEntity
            {
                Id = 3,
                UserId = 1,
                User = new UserEntity
                {
                    FirstName = "First",
                    LastName = "Last",
                    Email = "email",
                    DepartmentId = "Net",
                },
                CheckedAtUtc = exitUtc,
                Door = "Door",
                Direction = DirectionType.Exit,
                Location = LocationType.Vitebsk
            },
        };

        var expected = new List<OdpAttendanceTotal>()
        {
            new OdpAttendanceTotal
            {
                UserId = attendances[0].UserId,
                UserDepartmentId = attendances[0].User!.DepartmentId,
                UserFirstName = attendances[0].User!.FirstName,
                UserLastName = attendances[0].User!.LastName,
                UserFullName = ToFullName.Reverse(attendances[0].User!.FirstName, attendances[0].User!.LastName),
                Date = DateOnly.FromDateTime(enterUtc.Date),
                TotalTimeInMinutes = durationToMidnightVitebskLocal
            },
            new OdpAttendanceTotal
            {
                UserId = attendances[0].UserId,
                UserDepartmentId = attendances[0].User!.DepartmentId,
                UserFirstName = attendances[0].User!.FirstName,
                UserLastName = attendances[0].User!.LastName,
                UserFullName = ToFullName.Reverse(attendances[0].User!.FirstName, attendances[0].User!.LastName),
                Date = DateOnly.FromDateTime(exitUtc.Date),
                TotalTimeInMinutes = durationFromMidnightVitebskLocal
            },
        };

        // Act
        var result = _calculator.Calculate(attendances);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(expected);
    }
}
