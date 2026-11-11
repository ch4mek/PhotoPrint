using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.ProjectExits.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.ProjectExits.Requests;

public class RestoreProjectExitRequestTests
{
    private readonly Mock<ITeamBoardDbContext> _dbContextMock = new();
    private readonly RestoreProjectExitRequest _handler;
    private readonly CancellationToken _token = CancellationToken.None;

    public RestoreProjectExitRequestTests()
    {
        _handler = new RestoreProjectExitRequest(_dbContextMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowEntityNotFoundException_WhenProjectExitNotFound()
    {
        // Arrange
        _dbContextMock.Setup(x => x.ProjectExits)
            .Returns(new List<ProjectExit>().BuildMockDbSet().Object);

        var command = new RestoreProjectExitRequest.Command(Id: 99);

        // Act
        var act = () => _handler.Handle(command, _token);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowEntityNotFoundException_WhenProjectExitIsAlreadyActive()
    {
        // Arrange
        _dbContextMock.Setup(x => x.ProjectExits)
            .Returns(new List<ProjectExit>
            {
                new() { Id = 1, IsActive = true }
            }.BuildMockDbSet().Object);

        var command = new RestoreProjectExitRequest.Command(Id: 1);

        // Act
        var act = () => _handler.Handle(command, _token);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldSetIsActiveToTrue_AndReturnDto()
    {
        // Arrange
        var department = new TeamboardDepartment { DepartmentId = "NET", ShortName = "net", FullName = ".NET" };
        var resourceGroup = new TeamBoardResourceGroup { ResourceGroupName = "G1" };
        var closingReason = new ProjectExitClosingReason { Id = 1, Name = "End of project" };

        var exit = new ProjectExit
        {
            Id = 1,
            UserId = 2,
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
            ClosingReasonId = 1,
            ClosingReason = closingReason,
            IsActive = false,
            CreatedAtUtc = DateTime.UtcNow
        };

        _dbContextMock.Setup(x => x.ProjectExits)
            .Returns(new List<ProjectExit> { exit }.BuildMockDbSet().Object);

        var command = new RestoreProjectExitRequest.Command(Id: 1);

        // Act
        var result = await _handler.Handle(command, _token);

        // Assert
        exit.IsActive.Should().BeTrue();
        _dbContextMock.Verify(x => x.SaveChangesAsync(_token), Times.Once);
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.UserId.Should().Be(2);
        result.IsActive.Should().BeTrue();
    }
}
