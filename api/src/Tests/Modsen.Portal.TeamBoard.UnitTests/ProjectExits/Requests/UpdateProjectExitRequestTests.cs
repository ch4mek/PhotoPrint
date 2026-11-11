using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.ProjectExits.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.ProjectExits.Requests;

public class UpdateProjectExitRequestTests
{
    private readonly Mock<ITeamBoardDbContext> _dbContextMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly UpdateProjectExitRequest _handler;
    private readonly CancellationToken _token = CancellationToken.None;

    public UpdateProjectExitRequestTests()
    {
        _handler = new UpdateProjectExitRequest(
            _dbContextMock.Object,
            _authenticatedUserContextMock.Object);

        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(1);
    }

    [Fact]
    public async Task Handle_ShouldThrowEntityNotFoundException_WhenProjectExitNotFound()
    {
        // Arrange
        _dbContextMock.Setup(x => x.ProjectExits)
            .Returns(new List<ProjectExit>().BuildMockDbSet().Object);

        var command = new UpdateProjectExitRequest.Command(
            Id: 99,
            UserId: 1,
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
    public async Task Handle_ShouldThrowEntityNotFoundException_WhenProjectExitIsInactive()
    {
        // Arrange
        _dbContextMock.Setup(x => x.ProjectExits)
            .Returns(new List<ProjectExit>
            {
                new() { Id = 1, IsActive = false }
            }.BuildMockDbSet().Object);

        var command = new UpdateProjectExitRequest.Command(
            Id: 1,
            UserId: 1,
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
    public async Task Handle_ShouldThrowEntityNotFoundException_WhenUserNotFound()
    {
        // Arrange
        _dbContextMock.Setup(x => x.ProjectExits)
            .Returns(new List<ProjectExit>
            {
                new() { Id = 1, IsActive = true }
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>().BuildMockDbSet().Object);

        var command = new UpdateProjectExitRequest.Command(
            Id: 1,
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
        _dbContextMock.Setup(x => x.ProjectExits)
            .Returns(new List<ProjectExit>
            {
                new() { Id = 1, IsActive = true }
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                new() { UserId = 1, FirstName = "Test", LastName = "User", Email = "test@test.com" }
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.TeamBoardProjects)
            .Returns(new List<TeamBoardProject>().BuildMockDbSet().Object);

        var command = new UpdateProjectExitRequest.Command(
            Id: 1,
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
    public async Task Handle_ShouldUpdateProjectExit_WithExistingClosingReason()
    {
        // Arrange
        var closingReason = new ProjectExitClosingReason { Id = 3, Name = "Contract ended" };
        var department = new TeamboardDepartment { DepartmentId = "NET", ShortName = "net", FullName = ".NET" };
        var resourceGroup = new TeamBoardResourceGroup { ResourceGroupName = "G1" };

        var exit = new ProjectExit
        {
            Id = 1,
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
            ClosingDate = new DateOnly(2025, 1, 1),
            ClosingReasonId = 3,
            ClosingReason = closingReason,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        _dbContextMock.Setup(x => x.ProjectExits)
            .Returns(new List<ProjectExit> { exit }.BuildMockDbSet().Object);

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

        _dbContextMock.Setup(x => x.ProjectExitClosingReasons)
            .Returns(new List<ProjectExitClosingReason> { closingReason }.BuildMockDbSet().Object);

        var command = new UpdateProjectExitRequest.Command(
            Id: 1,
            UserId: 1,
            ProjectId: 1,
            ClientId: 1,
            ClosingDate: new DateOnly(2025, 6, 1),
            ClosingReasonName: "Contract ended");

        // Act
        var result = await _handler.Handle(command, _token);

        // Assert
        _dbContextMock.Verify(x => x.SaveChangesAsync(_token), Times.Once);
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.ClosingDate.Should().Be(new DateOnly(2025, 6, 1));
        result.ClosingReasonId.Should().Be(3);
    }

    [Fact]
    public async Task Handle_ShouldUpdateProjectExit_WithNewClosingReason()
    {
        // Arrange
        var existingReason = new ProjectExitClosingReason { Id = 2, Name = "Old reason" };
        var newReason = new ProjectExitClosingReason { Id = 0, Name = "Brand new reason" };
        var department = new TeamboardDepartment { DepartmentId = "JS", ShortName = "js", FullName = "JavaScript" };
        var resourceGroup = new TeamBoardResourceGroup { ResourceGroupName = "JS1" };

        var exit = new ProjectExit
        {
            Id = 1,
            UserId = 1,
            User = new TeamBoardUser
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                DepartmentId = "JS",
                Department = department,
                ResourceGroupId = 2,
                ResourceGroup = resourceGroup,
                IsActive = true
            },
            ClientId = 1,
            Client = new TeamBoardClient { ClientId = 1, Name = "Modsen" },
            ProjectId = 1,
            Project = new TeamBoardProject { Id = 1, ProjectName = "Project1", ClientName = "Modsen" },
            ClosingDate = new DateOnly(2025, 1, 1),
            ClosingReasonId = 2,
            ClosingReason = newReason,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        _dbContextMock.Setup(x => x.ProjectExits)
            .Returns(new List<ProjectExit> { exit }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                new() { UserId = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com" }
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.TeamBoardProjects)
            .Returns(new List<TeamBoardProject>
            {
                new() { Id = 1, ProjectName = "Project1", ClientName = "Modsen" }
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.ProjectExitClosingReasons)
            .Returns(new List<ProjectExitClosingReason> { existingReason }.BuildMockDbSet().Object);

        var command = new UpdateProjectExitRequest.Command(
            Id: 1,
            UserId: 1,
            ProjectId: 1,
            ClientId: 1,
            ClosingDate: new DateOnly(2025, 6, 1),
            ClosingReasonName: "Brand new reason");

        // Act
        var result = await _handler.Handle(command, _token);

        // Assert
        _dbContextMock.Verify(x => x.ProjectExitClosingReasons.AddAsync(
            It.Is<ProjectExitClosingReason>(r => r.Name == "Brand new reason"), _token), Times.Once);
        _dbContextMock.Verify(x => x.SaveChangesAsync(_token), Times.AtLeast(2));
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
    }
}
