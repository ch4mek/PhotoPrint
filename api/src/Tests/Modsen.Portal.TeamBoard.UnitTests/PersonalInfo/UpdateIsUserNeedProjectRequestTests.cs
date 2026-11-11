using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.PersonalInfo;

public class UpdateIsUserNeedProjectRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authContextMock = new();
    private readonly Mock<ITeamBoardDbContext> _dbContextMock = new();
    private readonly UpdateIsUserNeedProjectRequest _handler;

    public UpdateIsUserNeedProjectRequestTests()
    {
        _handler = new UpdateIsUserNeedProjectRequest(
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
            new UpdateIsUserNeedProjectRequest.Command(1, true),
            CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleShouldThrowWhenHeadOfDepartmentFromDifferentDepartment()
    {
        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                CreateUser(1, "Java")
            }.BuildMockDbSet().Object);

        _authContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        _authContextMock.Setup(x => x.DepartmentId).Returns("Net");
        _authContextMock.Setup(x => x.GetRequiredUserId()).Returns(999);

        var act = async () => await _handler.Handle(
            new UpdateIsUserNeedProjectRequest.Command(1, true),
            CancellationToken.None);

        await act.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task HandleShouldThrowWhenGroupManagerFromDifferentGroup()
    {
        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                CreateUser(1, "Net", groupId: 2)
            }.BuildMockDbSet().Object);

        _authContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.GroupManager]);

        _authContextMock.Setup(x => x.ResourceGroupId).Returns(1);
        _authContextMock.Setup(x => x.GetRequiredUserId()).Returns(999);

        var act = async () => await _handler.Handle(
            new UpdateIsUserNeedProjectRequest.Command(1, true),
            CancellationToken.None);

        await act.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task HandleShouldThrowWhenUserTriesToUpdateSelf()
    {
        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                CreateUser(1)
            }.BuildMockDbSet().Object);

        _authContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee]);

        _authContextMock.Setup(x => x.GetRequiredUserId()).Returns(1);

        var act = async () => await _handler.Handle(
            new UpdateIsUserNeedProjectRequest.Command(1, true),
            CancellationToken.None);

        await act.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task HandleShouldUpdateExistingPayload()
    {
        var payload = new UserPayloadDetails
        {
            UserId = 1,
            NeedProject = false
        };

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                CreateUser(1)
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.UserPayloadDetails)
            .Returns(new List<UserPayloadDetails>
            {
                payload
            }.BuildMockDbSet().Object);

        _authContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee]);

        _authContextMock.Setup(x => x.GetRequiredUserId()).Returns(999);
        _authContextMock.Setup(x => x.UserId).Returns(100);

        _dbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new UpdateIsUserNeedProjectRequest.Command(1, true),
            CancellationToken.None);

        result.Should().BeTrue();
        payload.NeedProject.Should().BeTrue();
        payload.UpdatedById.Should().Be(100);
    }

    [Fact]
    public async Task HandleShouldCreatePayloadWhenNotExists()
    {
        var payloadList = new List<UserPayloadDetails>();

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                CreateUser(1)
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.UserPayloadDetails)
            .Returns(payloadList.BuildMockDbSet().Object);

        _authContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee]);

        _authContextMock.Setup(x => x.GetRequiredUserId()).Returns(999);
        _authContextMock.Setup(x => x.UserId).Returns(100);

        _dbContextMock.Setup(x => x.UserPayloadDetails.Add(It.IsAny<UserPayloadDetails>()))
            .Callback<UserPayloadDetails>(payloadList.Add);

        _dbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new UpdateIsUserNeedProjectRequest.Command(1, true),
            CancellationToken.None);

        result.Should().BeTrue();

        payloadList.Should().ContainSingle();
        payloadList[0].NeedProject.Should().BeTrue();
        payloadList[0].CreatedById.Should().Be(100);
    }

    [Fact]
    public async Task HandleShouldReturnValueFromRequest()
    {
        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                CreateUser(1)
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.UserPayloadDetails)
            .Returns(new List<UserPayloadDetails>
            {
                new() { UserId = 1 }
            }.BuildMockDbSet().Object);

        _authContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee]);

        _authContextMock.Setup(x => x.GetRequiredUserId()).Returns(999);
        _authContextMock.Setup(x => x.UserId).Returns(100);

        _dbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new UpdateIsUserNeedProjectRequest.Command(1, false),
            CancellationToken.None);

        result.Should().BeFalse();
    }
}
