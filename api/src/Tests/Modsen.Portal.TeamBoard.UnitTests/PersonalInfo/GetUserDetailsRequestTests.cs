using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.PersonalInfo;

public class GetUserDetailsRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authContextMock = new();
    private readonly Mock<ITeamBoardDbContext> _dbContextMock = new();
    private readonly GetUserDetailsRequest _handler;

    public GetUserDetailsRequestTests()
    {
        _handler = new GetUserDetailsRequest(
            _dbContextMock.Object,
            _authContextMock.Object
        );
    }

    [Fact]
    public async Task HandleShouldThrowWhenUserNotFound()
    {
        _dbContextMock
            .Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>().BuildMockDbSet().Object);

        var act = async () => await _handler.Handle(
            new GetUserDetailsRequest.Query(1),
            CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleShouldReturnPlanWhenSuccess()
    {
        _dbContextMock
            .Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                new()
                {
                    FirstName = "Test",
                    LastName = "Test",
                    Email = "test",
                    UserId = 1,
                    DepartmentId = "Net",
                    ResourceGroupId = 1,
                    UserPayloadDetails =
                    [
                        new()
                        {
                            Plan = "test-plan"
                        }
                    ]
                }
            }.BuildMockDbSet().Object);

        _authContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee]);

        _authContextMock.Setup(x => x.UserId).Returns(999);

        var result = await _handler.Handle(
            new GetUserDetailsRequest.Query(1),
            CancellationToken.None);

        result.Plan.Should().Be("test-plan");
    }

    [Fact]
    public async Task HandleShouldThrowWhenHeadOfDepartmentFromAnotherDepartment()
    {
        _dbContextMock
            .Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                new()
                {
                    UserId = 1,
                    DepartmentId = "Java",
                    ResourceGroupId = 1,
                    FirstName = "Test",
                    LastName = "Test",
                    Email = "test",
                }
            }.BuildMockDbSet().Object);

        _authContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        _authContextMock.Setup(x => x.DepartmentId).Returns("Net");
        _authContextMock.Setup(x => x.UserId).Returns(999);

        var act = async () => await _handler.Handle(
            new GetUserDetailsRequest.Query(1),
            CancellationToken.None);

        await act.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task HandleShouldThrowWhenGroupManagerFromAnotherGroup()
    {
        _dbContextMock
            .Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                new()
                {
                    FirstName = "Test",
                    LastName = "Test",
                    Email = "test",
                    UserId = 1,
                    DepartmentId = "Net",
                    ResourceGroupId = 2
                }
            }.BuildMockDbSet().Object);

        _authContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.GroupManager]);

        _authContextMock.Setup(x => x.ResourceGroupId).Returns(1);
        _authContextMock.Setup(x => x.UserId).Returns(999);

        var act = async () => await _handler.Handle(
            new GetUserDetailsRequest.Query(1),
            CancellationToken.None);

        await act.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task HandleShouldThrowWhenUserTriesToViewOwnPlan()
    {
        _dbContextMock
            .Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                new()
                {
                    FirstName = "Test",
                    LastName = "Test",
                    Email = "test",
                    UserId = 1,
                    DepartmentId = "Net",
                    ResourceGroupId = 1
                }
            }.BuildMockDbSet().Object);

        _authContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee]);

        _authContextMock.Setup(x => x.UserId).Returns(1);

        var act = async () => await _handler.Handle(
            new GetUserDetailsRequest.Query(1),
            CancellationToken.None);

        await act.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task HandleShouldReturnNullPlanWhenNoPayloadDetails()
    {
        _dbContextMock
            .Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                new()
                {
                    FirstName = "Test",
                    LastName = "Test",
                    Email = "test",
                    UserId = 1,
                    DepartmentId = "Net",
                    ResourceGroupId = 1,
                    UserPayloadDetails = []
                }
            }.BuildMockDbSet().Object);

        _authContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee]);

        _authContextMock.Setup(x => x.UserId).Returns(999);

        var result = await _handler.Handle(
            new GetUserDetailsRequest.Query(1),
            CancellationToken.None);

        result.Plan.Should().BeNull();
    }
}
