using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Users.Requests;
using Modsen.Portal.TimeTrack.Contract;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.PersonalInfo;

public class GetPermanentUserInfoRequestTests
{
    private readonly Mock<ITeamBoardDbContext> _dbContextMock = new();
    private readonly Mock<IIdentityContract> _identityMock = new();
    private readonly Mock<ITimeTrackContract> _timeTrackContract = new();

    private readonly GetPermanentUserInfoRequest _handler;

    public GetPermanentUserInfoRequestTests()
    {
        _handler = new GetPermanentUserInfoRequest(
            _dbContextMock.Object,
            _identityMock.Object,
            _timeTrackContract.Object);
    }

    private static TeamBoardUser CreateUser(int id, string? role = null)
    {
        return new TeamBoardUser
        {
            UserId = id,
            FirstName = $"Name{id}",
            LastName = $"Last{id}",
            Email = $"user{id}@mail.com",
            DepartmentId = "Net",
            Position = role ?? PortalRoleNames.Employee,
            Contracts = [],
            ResourceGroup = new TeamBoardResourceGroup
            {
                ResourceGroupName = "Group 1"
            },
            UserSpecializations = [],
            UserProjects = [],
        };
    }

    [Fact]
    public async Task HandleShouldThrowWhenUserNotFound()
    {
        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>().BuildMockDbSet().Object);

        var act = async () => await _handler.Handle(
            new GetPermanentUserInfoRequest.Query("user1@mail.com"),
            CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleShouldReturnUserInfo()
    {
        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser> { CreateUser(1) }.BuildMockDbSet().Object);

        _identityMock.Setup(x => x.GetHigherManagerIds(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _handler.Handle(
            new GetPermanentUserInfoRequest.Query("user1@mail.com"),
            CancellationToken.None);

        result.FirstName.Should().Be("Name1");
        result.Email.Should().Be("user1@mail.com");
        result.ResourceGroupName.Should().Be("Group 1");
    }

    [Fact]
    public async Task HandleShouldReturnManagers()
    {
        var user = CreateUser(1);
        var manager1 = CreateUser(2);
        var manager2 = CreateUser(3);

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                user, manager1, manager2
            }.BuildMockDbSet().Object);

        _identityMock.Setup(x => x.GetHigherManagerIds(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([ 2, 3 ]);

        var result = await _handler.Handle(
            new GetPermanentUserInfoRequest.Query("user1@mail.com"),
            CancellationToken.None);

        result.Managers.Should().HaveCount(2);
        result.Managers.Select(x => x.FirstName)
            .Should().BeEquivalentTo("Name2", "Name3");
    }

    [Fact]
    public async Task HandleShouldReturnEmptyManagersWhenNoMatches()
    {
        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser> { CreateUser(1) }.BuildMockDbSet().Object);

        _identityMock.Setup(x => x.GetHigherManagerIds(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([999, 888 ]);

        var result = await _handler.Handle(
            new GetPermanentUserInfoRequest.Query("user1@mail.com"),
            CancellationToken.None);

        result.Managers.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleShouldReturnEmptyManagersWhenManagerIdsEmpty()
    {
        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser> { CreateUser(1) }.BuildMockDbSet().Object);

        _identityMock.Setup(x => x.GetHigherManagerIds(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _handler.Handle(
            new GetPermanentUserInfoRequest.Query("user1@mail.com"),
            CancellationToken.None);

        result.Managers.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleShouldReturnManagersForEmployee()
    {
        var user = CreateUser(1);
        var gm = CreateUser(2, PortalRoleNames.GroupManager);

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser> { user, gm }.BuildMockDbSet().Object);

        _identityMock.Setup(x => x.GetHigherManagerIds(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([2]);

        _timeTrackContract.Setup(x => x.GetProjectManagerIdsByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _handler.Handle(
            new GetPermanentUserInfoRequest.Query(user.Email),
            CancellationToken.None);

        result.Managers.Should().HaveCount(1);
        result.Managers.First().FirstName.Should().Be(gm.FirstName);
    }

    [Fact]
    public async Task HandleShouldIncludeProjectManagers()
    {
        var user = CreateUser(1);
        var pm = CreateUser(3);

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>{ user, pm}.BuildMockDbSet().Object);

        _identityMock.Setup(x => x.GetHigherManagerIds(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        _timeTrackContract.Setup(x => x.GetProjectManagerIdsByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([3]);

        var result = await _handler.Handle(
            new GetPermanentUserInfoRequest.Query(user.Email),
            CancellationToken.None);

        result.Managers.Should().HaveCount(1);
        result.Managers.First().Position.Should().Be("ProjectManager");
    }

    [Fact]
    public async Task HandleShouldReturnChiefForHeadOfDepartment()
    {
        var user = CreateUser(1, PortalRoleNames.HeadOfDepartment);
        var chief = CreateUser(10, PortalRoleNames.Cto);

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser> { user, chief }.BuildMockDbSet().Object);

        _identityMock.Setup(x => x.GetUserChiefIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);

        _timeTrackContract.Setup(x => x.GetProjectManagerIdsByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _handler.Handle(
            new GetPermanentUserInfoRequest.Query(user.Email),
            CancellationToken.None);

        result.Managers.Should().HaveCount(1);
        result.Managers.First().FirstName.Should().Be(chief.FirstName);
    }

    [Fact]
    public async Task HandleShouldReturnCeoForCto()
    {
        var user = CreateUser(1, PortalRoleNames.Cto);
        var ceo = CreateUser(99, PortalRoleNames.Ceo);

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser> { user, ceo }.BuildMockDbSet().Object);

        _timeTrackContract.Setup(x => x.GetProjectManagerIdsByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _handler.Handle(
            new GetPermanentUserInfoRequest.Query(user.Email),
            CancellationToken.None);

        result.Managers.Should().HaveCount(1);
        result.Managers.First().Position.Should().Be(PortalRoleNames.Ceo);
    }

    [Fact]
    public async Task HandleShouldCombineManagersAndProjectManagers()
    {
        var user = CreateUser(1, PortalRoleNames.Employee);
        var manager = CreateUser(2, "Manager");
        var pm = CreateUser(3, "Whatever");

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser> { user, manager, pm }.BuildMockDbSet().Object);

        _identityMock.Setup(x => x.GetHigherManagerIds(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([2]);

        _timeTrackContract.Setup(x => x.GetProjectManagerIdsByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([3]);

        var result = await _handler.Handle(
            new GetPermanentUserInfoRequest.Query(user.Email),
            CancellationToken.None);

        result.Managers.Should().HaveCount(2);
    }

    [Fact]
    public async Task HandleShouldReturnEmptyWhenNoManagers()
    {
        var user = CreateUser(1, PortalRoleNames.Employee);

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser> { user }.BuildMockDbSet().Object);

        _identityMock.Setup(x => x.GetHigherManagerIds(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        _timeTrackContract.Setup(x => x.GetProjectManagerIdsByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _handler.Handle(
            new GetPermanentUserInfoRequest.Query(user.Email),
            CancellationToken.None);

        result.Managers.Should().BeEmpty();
    }
}
