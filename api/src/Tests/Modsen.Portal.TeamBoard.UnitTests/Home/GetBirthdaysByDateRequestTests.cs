using MockQueryable.Moq;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Home.Core.Converters;
using Modsen.Portal.TeamBoard.Requests.Home.Requests;
using Modsen.Portal.TeamBoard.Requests.Home.Requests.RequestsDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Home;

public class GetBirthdaysByDateRequestTests
{
    private readonly Mock<ITeamBoardDbContext> _dbContextMock = new();

    private readonly GetBirthdaysByDateRequest _handler;
    private readonly CancellationToken _token = CancellationToken.None;

    public GetBirthdaysByDateRequestTests()
    {
        _handler = new GetBirthdaysByDateRequest(
            _dbContextMock.Object,
            new TeamboardUserToDepartmentNameConverter());
    }

    [Fact]
    public async Task HandleReturnsValidBirthdays()
    {
        //Arrange
        var ceoUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.Ceo,
            FirstName = "CEO first name",
            LastName = "CEO last name",
            Email = "test",
            AvatarFileId = "filed",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-25)),
            Department = null
        };

        var ctoUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.Cto,
            FirstName = "CTO first name",
            LastName = "CTO last name",
            Email = "test",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-30)),
            Department = null
        };

        var headUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.HeadOfDepartment,
            FirstName = "Head first name",
            LastName = "Head last name",
            Email = "test",
            AvatarFileId = "filed",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-20)),
            Department = new TeamboardDepartment { FullName = ".NET", DepartmentId = "Net", ShortName = ".NET" }
        };

        var groupManagerUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.GroupManager,
            FirstName = "GroupManager first name",
            LastName = "GroupManager last name",
            Email = "test",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-18)),
            Department = new TeamboardDepartment { FullName = ".NET", DepartmentId = "Net", ShortName = ".NET" }
        };

        var sysAdminUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.SystemAdministrator,
            FirstName = "SysAdmin first name",
            LastName = "SysAdmin last name",
            Email = "test",
            AvatarFileId = "filed",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-40)),
            Department = new TeamboardDepartment { FullName = "System Administrators", DepartmentId = "Net", ShortName = ".NET" }
        };

        var employeeUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.Employee,
            FirstName = "Employee first name",
            LastName = "Employee last name",
            Email = "test",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-33)),
            Department = new TeamboardDepartment { FullName = "DevOps", DepartmentId = "Net", ShortName = ".NET" }
        };

        _dbContextMock
            .Setup(context => context.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                ceoUser, ctoUser, headUser, groupManagerUser, sysAdminUser, employeeUser
            }.BuildMockDbSet().Object);

        var expectedOutput = new List<UserBirthdayDto>()
        {
            new()
            {
                LastName = ceoUser.LastName,
                FirstName = ceoUser.FirstName,
                AvatarFileId = ceoUser.AvatarFileId,
                Department = PortalRoleNames.Ceo,
            },
            new()
            {
                LastName = ctoUser.LastName,
                FirstName = ctoUser.FirstName,
                AvatarFileId = ctoUser.AvatarFileId,
                Department = PortalRoleNames.Cto,
            },
            new()
            {
                LastName = headUser.LastName,
                FirstName = headUser.FirstName,
                AvatarFileId = headUser.AvatarFileId,
                Department = $"Head of {headUser.Department.FullName}",
            },
            new()
            {
                LastName = groupManagerUser.LastName,
                FirstName = groupManagerUser.FirstName,
                AvatarFileId = groupManagerUser.AvatarFileId,
                Department = groupManagerUser.Department.FullName,
            },
            new()
            {
                LastName = employeeUser.LastName,
                FirstName = employeeUser.FirstName,
                AvatarFileId = employeeUser.AvatarFileId,
                Department = employeeUser.Department.FullName,
            },
            new()
            {
                LastName = sysAdminUser.LastName,
                FirstName = sysAdminUser.FirstName,
                AvatarFileId = sysAdminUser.AvatarFileId,
                Department = sysAdminUser.Department.FullName,
            }
        };

        //Act
        var birthdays = await _handler.Handle(
            new GetBirthdaysByDateRequest.Query(DateOnly.FromDateTime(DateTime.Today)), _token);

        //Assert
        birthdays.Should().BeEquivalentTo(expectedOutput);
    }

    [Fact]
    public async Task HandleWhenUserHasBirthdayAt29ThFebruaryAndNowIsNotLeapYear28ThFebruaryReturns28ThAnd29ThFebruaryBirthdays()
    {
        //Arrange
        var leapYear29ThFebruaryBirthday = new DateOnly(2004, 2, 29);
        var notLeapYear28ThFebruary = new DateOnly(2025, 2, 28);

        var ceoUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.Ceo,
            FirstName = "CEO first name",
            LastName = "CEO last name",
            Email = "test",
            AvatarFileId = "filed",
            DateOfBirth = notLeapYear28ThFebruary.AddYears(-25),
            Department = null
        };

        var ctoUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.Cto,
            FirstName = "CTO first name",
            LastName = "CTO last name",
            Email = "test",
            DateOfBirth = leapYear29ThFebruaryBirthday,
            Department = null
        };

        _dbContextMock
            .Setup(context => context.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                ceoUser, ctoUser,
            }.BuildMockDbSet().Object);

        var expectedOutput = new List<UserBirthdayDto>()
        {
            new()
            {
                LastName = ceoUser.LastName,
                FirstName = ceoUser.FirstName,
                AvatarFileId = ceoUser.AvatarFileId,
                Department = PortalRoleNames.Ceo,
            },
            new()
            {
                LastName = ctoUser.LastName,
                FirstName = ctoUser.FirstName,
                AvatarFileId = ctoUser.AvatarFileId,
                Department = PortalRoleNames.Cto,
            }
        };

        //Act
        var birthdays = await _handler.Handle(
            new GetBirthdaysByDateRequest.Query(notLeapYear28ThFebruary), _token);

        //Assert
        birthdays.Should().BeEquivalentTo(expectedOutput);
    }

    [Fact]
    public async Task HandleWhenUserHasBirthdayAt29ThFebruaryAndNowIsLeapYear29ThFebruaryReturns29ThFebruaryBirthdays()
    {
        //Arrange
        var leapYear29ThFebruaryBirthday = new DateOnly(2004, 2, 29);

        var ceoUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.Ceo,
            FirstName = "CEO first name",
            LastName = "CEO last name",
            Email = "test",
            AvatarFileId = "filed",
            DateOfBirth = leapYear29ThFebruaryBirthday,
            Department = null
        };

        var ctoUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.Cto,
            FirstName = "CTO first name",
            LastName = "CTO last name",
            Email = "test",
            DateOfBirth = leapYear29ThFebruaryBirthday,
            Department = null
        };

        _dbContextMock
            .Setup(context => context.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                ceoUser, ctoUser,
            }.BuildMockDbSet().Object);

        var expectedOutput = new List<UserBirthdayDto>()
        {
            new()
            {
                LastName = ceoUser.LastName,
                FirstName = ceoUser.FirstName,
                AvatarFileId = ceoUser.AvatarFileId,
                Department = PortalRoleNames.Ceo,
            },
            new()
            {
                LastName = ctoUser.LastName,
                FirstName = ctoUser.FirstName,
                AvatarFileId = ctoUser.AvatarFileId,
                Department = PortalRoleNames.Cto,
            }
        };

        //Act
        var birthdays = await _handler.Handle(
            new GetBirthdaysByDateRequest.Query(new DateOnly(2024, 2, 29)), _token);

        //Assert
        birthdays.Should().BeEquivalentTo(expectedOutput);
    }
}
