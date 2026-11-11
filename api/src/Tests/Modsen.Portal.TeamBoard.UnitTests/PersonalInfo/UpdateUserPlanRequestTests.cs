using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.PersonalInfo;

public class UpdateUserPlanRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authContextMock = new();
    private readonly Mock<ITeamBoardDbContext> _dbContextMock = new();
    private readonly UpdateUserPlanRequest _handler;

    public UpdateUserPlanRequestTests()
    {
        _handler = new UpdateUserPlanRequest(
            _dbContextMock.Object,
            _authContextMock.Object);
    }

    private static TeamBoardUser CreateUser(
        int userId,
        string department = "Net",
        int groupId = 1,
        bool isActive = true)
    {
        return new TeamBoardUser
        {
            FirstName = "Test",
            LastName = "Test",
            Email = "Test",
            UserId = userId,
            DepartmentId = department,
            ResourceGroupId = groupId,
            IsActive = isActive
        };
    }

    [Fact]
    public async Task HandleShouldThrowWhenUserNotFound()
    {
        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>().BuildMockDbSet().Object);

        var act = async () => await _handler.Handle(
            new UpdateUserPlanRequest.Command(1, "plan"),
            CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleShouldThrowWhenHeadOfDepartmentDifferentDepartment()
    {
        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                CreateUser(1, "Java")
            }.BuildMockDbSet().Object);

        _authContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        _authContextMock.Setup(x => x.DepartmentId).Returns("Net");
        _authContextMock.Setup(x => x.UserId).Returns(999);

        var act = async () => await _handler.Handle(
            new UpdateUserPlanRequest.Command(1, "plan"),
            CancellationToken.None);

        await act.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task HandleShouldThrowWhenGroupManagerDifferentGroup()
    {
        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                CreateUser(1, "Net", 2)
            }.BuildMockDbSet().Object);

        _authContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.GroupManager]);

        _authContextMock.Setup(x => x.ResourceGroupId).Returns(1);
        _authContextMock.Setup(x => x.UserId).Returns(999);

        var act = async () => await _handler.Handle(
            new UpdateUserPlanRequest.Command(1, "plan"),
            CancellationToken.None);

        await act.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task HandleShouldThrowWhenUserUpdatesOwnPlan()
    {
        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                CreateUser(1)
            }.BuildMockDbSet().Object);

        _authContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee]);

        _authContextMock.Setup(x => x.UserId).Returns(1);

        var act = async () => await _handler.Handle(
            new UpdateUserPlanRequest.Command(1, "plan"),
            CancellationToken.None);

        await act.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task HandleShouldUpdateExistingPlan()
    {
        var payload = new UserPayloadDetails
        {
            UserId = 1,
            Plan = "old"
        };

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser> { CreateUser(1) }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.UserPayloadDetails)
            .Returns(new List<UserPayloadDetails> { payload }.BuildMockDbSet().Object);

        _authContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee]);

        _authContextMock.Setup(x => x.UserId).Returns(100);

        _dbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new UpdateUserPlanRequest.Command(1, "new-plan"),
            CancellationToken.None);

        result.Should().Be("new-plan");
        payload.Plan.Should().Be("new-plan");
        payload.UpdatedById.Should().Be(100);
    }

    [Fact]
    public async Task HandleShouldCreatePlanWhenNotExists()
    {
        var payloadList = new List<UserPayloadDetails>();

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser> { CreateUser(1) }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.UserPayloadDetails)
            .Returns(payloadList.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.UserPayloadDetails.Add(It.IsAny<UserPayloadDetails>()))
            .Callback<UserPayloadDetails>(payloadList.Add);

        _authContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee]);

        _authContextMock.Setup(x => x.UserId).Returns(100);

        _dbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new UpdateUserPlanRequest.Command(1, "plan"),
            CancellationToken.None);

        result.Should().Be("plan");

        payloadList.Should().ContainSingle();
        payloadList[0].Plan.Should().Be("plan");
        payloadList[0].CreatedById.Should().Be(100);
    }
}