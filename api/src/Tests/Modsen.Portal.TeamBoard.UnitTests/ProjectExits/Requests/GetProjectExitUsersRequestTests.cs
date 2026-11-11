using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.ProjectExits.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.ProjectExits.Requests;

public class GetProjectExitUsersRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<ITeamBoardDbContext> _teamBoardDbContextMock = new();
    private readonly CancellationToken _cancellationToken;
    private readonly GetProjectExitUsersRequest _handler;

    public GetProjectExitUsersRequestTests()
    {
        _cancellationToken = CancellationToken.None;
        _handler = new GetProjectExitUsersRequest(
            _authenticatedUserContextMock.Object,
            _teamBoardDbContextMock.Object);
    }

    [Fact]
    public async Task GetProjectExitUsersLookupWhenUserIsHeadOdDepartmentShouldReturnEmptyArray()
    {
        //Arrange
        var users = new List<TeamBoardUser>
        {
            new()
            {
                UserId = 1,
                FirstName = "Anna",
                LastName = "Doyle",
                AvatarColor = "#FFFFFF",
                Email = "example@example.com",
                IsActive = true
            },
            new()
            {
                UserId = 2,
                FirstName = "Veronica",
                LastName = "Smith",
                AvatarColor = "#FFFFFF",
                Email = "example@example.com",
                IsActive = true
            }
        };

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns("NET");

        _teamBoardDbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(users.BuildMockDbSet().Object);

        //Act
        var result = await _handler.Handle(
            new GetProjectExitUsersRequest.Query(false), _cancellationToken);

        //Assert
        result.Should().NotBeNull();

        result.Count.Should().Be(0);
    }

    [Fact]
    public async Task GetProjectExitUsersLookupWhenHasManageProjectExitsClaimShouldReturnAllUsers()
    {
        //Arrange
        var users = new List<TeamBoardUser>
        {
            new()
            {
                UserId = 1,
                FirstName = "Anna",
                LastName = "Doyle",
                AvatarColor = "#FFFFFF",
                Email = "example@example.com",
                IsActive = true
            },
            new()
            {
                UserId = 2,
                FirstName = "Veronica",
                LastName = "Smith",
                AvatarColor = "#FFFFFF",
                Email = "example@example.com",
                IsActive = true
            }
        };

        _authenticatedUserContextMock.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageAllProjectExits.Name)).Returns(true);

        _teamBoardDbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(users.BuildMockDbSet().Object);

        //Act
        var result = await _handler.Handle(
            new GetProjectExitUsersRequest.Query(false), _cancellationToken);

        //Assert
        result.Should().NotBeNull();

        result.Count.Should().Be(2);
    }

    [Fact]
    public async Task GetProjectExitUsersLookupWhenHasManageProjectExitsClaimAndIncludeInactiveTrueShouldReturnAllUsers()
    {
        //Arrange
        var users = new List<TeamBoardUser>
        {
            new()
            {
                UserId = 1,
                FirstName = "Anna",
                LastName = "Doyle",
                AvatarColor = "#FFFFFF",
                Email = "example@example.com",
                IsActive = false
            },
            new()
            {
                UserId = 2,
                FirstName = "Veronica",
                LastName = "Smith",
                AvatarColor = "#FFFFFF",
                Email = "example@example.com",
                IsActive = true
            }
        };

        _authenticatedUserContextMock.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageAllProjectExits.Name)).Returns(true);

        _teamBoardDbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(users.BuildMockDbSet().Object);

        //Act
        var result = await _handler.Handle(
            new GetProjectExitUsersRequest.Query(true), _cancellationToken);

        //Assert
        result.Should().NotBeNull();

        result.Count.Should().Be(2);
    }

    [Fact]
    public async Task GetProjectExitUsersLookupWhenHasManageProjectExitsClaimAndIncludeInactiveFalseShouldReturnActiveUsers()
    {
        //Arrange
        var users = new List<TeamBoardUser>
        {
            new()
            {
                UserId = 1,
                FirstName = "Anna",
                LastName = "Doyle",
                AvatarColor = "#FFFFFF",
                Email = "example@example.com",
                IsActive = false
            },
            new()
            {
                UserId = 2,
                FirstName = "Veronica",
                LastName = "Smith",
                AvatarColor = "#FFFFFF",
                Email = "example@example.com",
                IsActive = true
            }
        };

        _authenticatedUserContextMock.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageAllProjectExits.Name)).Returns(true);

        _teamBoardDbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(users.BuildMockDbSet().Object);

        //Act
        var result = await _handler.Handle(
            new GetProjectExitUsersRequest.Query(false), _cancellationToken);

        //Assert
        result.Should().NotBeNull();

        result.Count.Should().Be(1);
    }

    [Fact]
    public async Task GetProjectExitUsersLookupWhenUserIsHeadOfDepartmentHasViewClaimShouldReturnsOnlyDepartmentUsers()
    {
        //Arrange
        var users = new List<TeamBoardUser>
        {
            new()
            {
                UserId = 1,
                FirstName = "Anna",
                LastName = "Doyle",
                AvatarColor = "#FFFFFF",
                Email = "example@example.com",
                DepartmentId = "NET",
                IsActive = true
            },
            new()
            {
                UserId = 2,
                FirstName = "Veronica",
                LastName = "Smith",
                AvatarColor = "#FFFFFF",
                Email = "example@example.com",
                IsActive = true
            }
        };

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns("NET");
        _authenticatedUserContextMock.Setup(x => x.HasClaim(StaffingClaimTypeNames.ViewProjectExits.Name)).Returns(true);

        _teamBoardDbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(users.BuildMockDbSet().Object);

        //Act
        var result = await _handler.Handle(
            new GetProjectExitUsersRequest.Query(false), _cancellationToken);

        //Assert
        result.Should().NotBeNull();

        result.Count.Should().Be(1);
    }

    [Fact]
    public async Task GetProjectExitUsersLookupWhenUserIsHeadOfGroupHasViewClaimShouldReturnsOnlyGroupUsers()
    {
        //Arrange
        var users = new List<TeamBoardUser>
        {
            new()
            {
                UserId = 1,
                FirstName = "Anna",
                LastName = "Doyle",
                AvatarColor = "#FFFFFF",
                Email = "example@example.com",
                ResourceGroupId = 1,
                IsActive = true
            },
            new()
            {
                UserId = 2,
                FirstName = "Veronica",
                LastName = "Smith",
                AvatarColor = "#FFFFFF",
                Email = "example@example.com",
                IsActive = true
            }
        };

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContextMock.Setup(x => x.ResourceGroupId).Returns(1);
        _authenticatedUserContextMock.Setup(x => x.HasClaim(StaffingClaimTypeNames.ViewProjectExits.Name)).Returns(true);

        _teamBoardDbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(users.BuildMockDbSet().Object);

        //Act
        var result = await _handler.Handle(
            new GetProjectExitUsersRequest.Query(false), _cancellationToken);

        //Assert
        result.Should().NotBeNull();

        result.Count.Should().Be(1);
    }

    [Fact]
    public async Task GetProjectExitUsersLookupWhenUserIsHeadAOfDepartmentAndHeadOfGroupHasViewClaimShouldReturnsOnlyDepartmentUsers()
    {
        //Arrange
        var users = new List<TeamBoardUser>
        {
            new()
            {
                UserId = 1,
                FirstName = "Anna",
                LastName = "Doyle",
                AvatarColor = "#FFFFFF",
                Email = "example@example.com",
                DepartmentId = "NET",
                ResourceGroupId = 1,
                IsActive = true
            },
            new()
            {
                UserId = 2,
                FirstName = "Veronica",
                LastName = "Smith",
                AvatarColor = "#FFFFFF",
                DepartmentId = "NET",
                ResourceGroupId = 2,
                Email = "example@example.com",
                IsActive = true
            }
            ,
            new()
            {
                UserId = 3,
                FirstName = "Veronica",
                LastName = "Smith",
                AvatarColor = "#FFFFFF",
                Email = "example@example.com",
                IsActive = true
            }
        };

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.GroupManager, PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns("NET");
        _authenticatedUserContextMock.Setup(x => x.ResourceGroupId).Returns(1);
        _authenticatedUserContextMock.Setup(x => x.HasClaim(StaffingClaimTypeNames.ViewProjectExits.Name)).Returns(true);

        _teamBoardDbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(users.BuildMockDbSet().Object);

        //Act
        var result = await _handler.Handle(
            new GetProjectExitUsersRequest.Query(false), _cancellationToken);

        //Assert
        result.Should().NotBeNull();

        result.Count.Should().Be(2);
    }

    [Fact]
    public async Task GetProjectExitUsersLookupWhenUserIsCeoHasNoClaimsShouldReturnAllUsers()
    {
        //Arrange
        var users = new List<TeamBoardUser>
        {
            new()
            {
                UserId = 1,
                FirstName = "Anna",
                LastName = "Doyle",
                AvatarColor = "#FFFFFF",
                Email = "example@example.com",
                DepartmentId = "NET",
                ResourceGroupId = 1,
                IsActive = true
            },
            new()
            {
                UserId = 2,
                FirstName = "Veronica",
                LastName = "Smith",
                AvatarColor = "#FFFFFF",
                DepartmentId = "NET",
                ResourceGroupId = 2,
                Email = "example@example.com",
                IsActive = true
            }
            ,
            new()
            {
                UserId = 3,
                FirstName = "Veronica",
                LastName = "Smith",
                AvatarColor = "#FFFFFF",
                Email = "example@example.com",
                IsActive = true
            }
        };

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Ceo]);

        _teamBoardDbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(users.BuildMockDbSet().Object);

        //Act
        var result = await _handler.Handle(
            new GetProjectExitUsersRequest.Query(false), _cancellationToken);

        //Assert
        result.Should().NotBeNull();

        result.Count.Should().Be(3);
    }

    [Fact]
    public async Task GetProjectExitUsersLookupWhenUserIsHeadOfDepartmentHasManageClaimShouldReturnsAllUsers()
    {
        //Arrange
        var users = new List<TeamBoardUser>
        {
            new()
            {
                UserId = 1,
                FirstName = "Anna",
                LastName = "Doyle",
                AvatarColor = "#FFFFFF",
                Email = "example@example.com",
                DepartmentId = "NET",
                IsActive = true
            },
            new()
            {
                UserId = 2,
                FirstName = "Veronica",
                LastName = "Smith",
                AvatarColor = "#FFFFFF",
                Email = "example@example.com",
                IsActive = true
            }
        };

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns("NET");
        _authenticatedUserContextMock.Setup(x => x.HasClaim(StaffingClaimTypeNames.ViewProjectExits.Name)).Returns(true);
        _authenticatedUserContextMock.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageAllProjectExits.Name)).Returns(true);

        _teamBoardDbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(users.BuildMockDbSet().Object);

        //Act
        var result = await _handler.Handle(
            new GetProjectExitUsersRequest.Query(false), _cancellationToken);

        //Assert
        result.Should().NotBeNull();

        result.Count.Should().Be(2);
    }
}
