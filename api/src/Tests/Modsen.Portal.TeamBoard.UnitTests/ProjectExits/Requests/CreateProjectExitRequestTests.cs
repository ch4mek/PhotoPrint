using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.ProjectExits.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.ProjectExits.Requests;

public class CreateProjectExitRequestTests
{
    private readonly Mock<ITeamBoardDbContext> _dbContextMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly CreateProjectExitRequest _handler;
    private readonly CancellationToken _token = CancellationToken.None;

    public CreateProjectExitRequestTests()
    {
        _handler = new CreateProjectExitRequest(
            _dbContextMock.Object,
            _authenticatedUserContextMock.Object);

        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(1);
    }

    [Fact]
    public async Task Handle_ShouldThrowEntityNotFoundException_WhenUserNotFound()
    {
        // Arrange
        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>().BuildMockDbSet().Object);

        var command = new CreateProjectExitRequest.Command(
            UserId: 99,
            ProjectId: 1,
            ClientId: 1,
            ClosingDate: new DateOnly(2025, 6, 1),
            ClosingReasonName: "End of project");

        // Act
        var act = () => _handler.Handle(command, _token);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowEntityNotFoundException_WhenProjectNotFound()
    {
        // Arrange
        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                new() { UserId = 1, FirstName = "Test", LastName = "User", Email = "test@test.com" }
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.TeamBoardProjects)
            .Returns(new List<TeamBoardProject>().BuildMockDbSet().Object);

        var command = new CreateProjectExitRequest.Command(
            UserId: 1,
            ProjectId: 99,
            ClientId: 1,
            ClosingDate: new DateOnly(2025, 6, 1),
            ClosingReasonName: "End of project");

        // Act
        var act = () => _handler.Handle(command, _token);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowEntityNotFoundException_WhenClientNotFound()
    {
        // Arrange
        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                new() { UserId = 1, FirstName = "Test", LastName = "User", Email = "test@test.com" }
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.TeamBoardProjects)
            .Returns(new List<TeamBoardProject>
            {
                new() { Id = 1, ProjectName = "Project1", ClientName = "Client1" }
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.TeamBoardClients)
            .Returns(new List<TeamBoardClient>().BuildMockDbSet().Object);

        var command = new CreateProjectExitRequest.Command(
            UserId: 1,
            ProjectId: 1,
            ClientId: 99,
            ClosingDate: new DateOnly(2025, 6, 1),
            ClosingReasonName: "End of project");

        // Act
        var act = () => _handler.Handle(command, _token);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldCreateProjectExit_WithExistingClosingReason()
    {
        // Arrange
        var closingReason = new ProjectExitClosingReason { Id = 5, Name = "End of project" };
        var department = new TeamboardDepartment { DepartmentId = "NET", ShortName = "net", FullName = ".NET" };
        var resourceGroup = new TeamBoardResourceGroup { ResourceGroupName = "G1" };

        var projectExitForReturn = new ProjectExit
        {
            Id = 0,
            UserId = 1,
            User = new TeamBoardUser
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@test.com",
                DepartmentId = "NET",
                Department = department,
                ResourceGroupId = 1,
                ResourceGroup = resourceGroup,
                IsActive = true
            },
            ClientId = 1,
            Client = new TeamBoardClient { ClientId = 1, Name = "Modsen" },
            ProjectId = 1,
            Project = new TeamBoardProject { Id = 1, ProjectName = "Project1", ClientName = "Modsen" },
            ClosingDate = new DateOnly(2025, 6, 1),
            ClosingReasonId = 5,
            ClosingReason = closingReason,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                new() { UserId = 1, FirstName = "Test", LastName = "User", Email = "test@test.com" }
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.TeamBoardProjects)
            .Returns(new List<TeamBoardProject>
            {
                new() { Id = 1, ProjectName = "Project1", ClientName = "Modsen" }
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.TeamBoardClients)
            .Returns(new List<TeamBoardClient>
            {
                new() { ClientId = 1, Name = "Modsen" }
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.ProjectExitClosingReasons)
            .Returns(new List<ProjectExitClosingReason> { closingReason }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.ProjectExits)
            .Returns(new List<ProjectExit> { projectExitForReturn }.BuildMockDbSet().Object);

        var command = new CreateProjectExitRequest.Command(
            UserId: 1,
            ProjectId: 1,
            ClientId: 1,
            ClosingDate: new DateOnly(2025, 6, 1),
            ClosingReasonName: "End of project");

        // Act
        var result = await _handler.Handle(command, _token);

        // Assert
        _dbContextMock.Verify(x => x.ProjectExits.AddAsync(It.IsAny<ProjectExit>(), _token), Times.Once);
        _dbContextMock.Verify(x => x.SaveChangesAsync(_token), Times.Once);
        result.Should().NotBeNull();
        result.ClosingReasonId.Should().Be(5);
    }

    [Fact]
    public async Task Handle_ShouldCreateProjectExit_WithNewClosingReason()
    {
        // Arrange
        var department = new TeamboardDepartment { DepartmentId = "NET", ShortName = "net", FullName = ".NET" };
        var resourceGroup = new TeamBoardResourceGroup { ResourceGroupName = "G1" };
        var newClosingReason = new ProjectExitClosingReason { Id = 0, Name = "New reason" };

        var projectExitForReturn = new ProjectExit
        {
            Id = 0,
            UserId = 1,
            User = new TeamBoardUser
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@test.com",
                DepartmentId = "NET",
                Department = department,
                ResourceGroupId = 1,
                ResourceGroup = resourceGroup,
                IsActive = true
            },
            ClientId = 1,
            Client = new TeamBoardClient { ClientId = 1, Name = "Modsen" },
            ProjectId = 1,
            Project = new TeamBoardProject { Id = 1, ProjectName = "Project1", ClientName = "Modsen" },
            ClosingDate = new DateOnly(2025, 6, 1),
            ClosingReasonId = 0,
            ClosingReason = newClosingReason,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                new() { UserId = 1, FirstName = "Test", LastName = "User", Email = "test@test.com" }
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.TeamBoardProjects)
            .Returns(new List<TeamBoardProject>
            {
                new() { Id = 1, ProjectName = "Project1", ClientName = "Modsen" }
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.TeamBoardClients)
            .Returns(new List<TeamBoardClient>
            {
                new() { ClientId = 1, Name = "Modsen" }
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.ProjectExitClosingReasons)
            .Returns(new List<ProjectExitClosingReason>().BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.ProjectExits)
            .Returns(new List<ProjectExit> { projectExitForReturn }.BuildMockDbSet().Object);

        var command = new CreateProjectExitRequest.Command(
            UserId: 1,
            ProjectId: 1,
            ClientId: 1,
            ClosingDate: new DateOnly(2025, 6, 1),
            ClosingReasonName: "New reason");

        // Act
        var result = await _handler.Handle(command, _token);

        // Assert
        _dbContextMock.Verify(x => x.ProjectExitClosingReasons.AddAsync(
            It.Is<ProjectExitClosingReason>(r => r.Name == "New reason"), _token), Times.Once);
        _dbContextMock.Verify(x => x.ProjectExits.AddAsync(It.IsAny<ProjectExit>(), _token), Times.Once);
        _dbContextMock.Verify(x => x.SaveChangesAsync(_token), Times.AtLeast(2));
        result.Should().NotBeNull();
    }
}
