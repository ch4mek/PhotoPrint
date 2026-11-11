using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.ProjectExits.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.ProjectExits.Requests;

public class DeleteProjectExitRequestTests
{
    private readonly Mock<ITeamBoardDbContext> _dbContextMock = new();
    private readonly DeleteProjectExitRequest _handler;
    private readonly CancellationToken _token = CancellationToken.None;

    public DeleteProjectExitRequestTests()
    {
        _handler = new DeleteProjectExitRequest(_dbContextMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowEntityNotFoundException_WhenProjectExitNotFound()
    {
        // Arrange
        _dbContextMock.Setup(x => x.ProjectExits)
            .Returns(new List<ProjectExit>().BuildMockDbSet().Object);

        var command = new DeleteProjectExitRequest.Command(Id: 99);

        // Act
        var act = () => _handler.Handle(command, _token);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowEntityNotFoundException_WhenProjectExitIsAlreadyInactive()
    {
        // Arrange
        _dbContextMock.Setup(x => x.ProjectExits)
            .Returns(new List<ProjectExit>
            {
                new() { Id = 1, IsActive = false }
            }.BuildMockDbSet().Object);

        var command = new DeleteProjectExitRequest.Command(Id: 1);

        // Act
        var act = () => _handler.Handle(command, _token);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldSetIsActiveToFalse_AndReturnId()
    {
        // Arrange
        var exit = new ProjectExit { Id = 1, IsActive = true };

        _dbContextMock.Setup(x => x.ProjectExits)
            .Returns(new List<ProjectExit> { exit }.BuildMockDbSet().Object);

        var command = new DeleteProjectExitRequest.Command(Id: 1);

        // Act
        var result = await _handler.Handle(command, _token);

        // Assert
        exit.IsActive.Should().BeFalse();
        _dbContextMock.Verify(x => x.SaveChangesAsync(_token), Times.Once);
        result.Should().Be(1);
    }
}
