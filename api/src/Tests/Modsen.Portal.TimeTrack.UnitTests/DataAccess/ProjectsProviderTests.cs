using MockQueryable.Moq;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Providers;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.Providers;

namespace Modsen.Portal.TimeTrack.UnitTests.DataAccess;

public class ProjectsProviderTests
{
    private readonly Mock<ITimeTrackDbContext> _dbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly IProjectsProvider _projectsProvider;

    public ProjectsProviderTests()
    {
        _projectsProvider = new ProjectsProvider(_dbContextMock.Object);
    }

    [Fact]
    public async Task GetProjectByIdAsyncWhenProjectWithProvidedIdDoesNotExistReturnNull()
    {
        // Arrange
        var dbSetMock = Array.Empty<Project>().BuildMockDbSet();

        _dbContextMock.Setup(x => x.Projects)
            .Returns(dbSetMock.Object);

        const int projectId = 645;

        // Act
        var project = await _projectsProvider.GetProjectByIdAsync(
            projectId,
            _cancellationToken);

        // Assert
        project.Should().BeNull();
    }

    [Fact]
    public async Task GetProjectByIdAsyncWhenProjectWithProvidedIdDoesExistReturnProjectDomainModel()
    {
        // Arrange
        const int projectId = 645;

        var dbSetMock = Enumerable.Repeat(new Project { Id = projectId, Name = "test" }, 1)
            .ToArray()
            .BuildMockDbSet();

        _dbContextMock.Setup(x => x.Projects)
            .Returns(dbSetMock.Object);

        // Act
        var project = await _projectsProvider.GetProjectByIdAsync(
            projectId,
            _cancellationToken);

        // Assert
        project.Should().NotBeNull();
        project.Should().BeOfType<TimeTrack.Domain.Project>();
    }

    [Fact]
    public async Task GetProjectByNameAsyncWhenProjectWithProvidedNameDoesNotExistReturnNull()
    {
        // Arrange
        var dbSetMock = Array.Empty<Project>().BuildMockDbSet();

        _dbContextMock.Setup(x => x.Projects)
            .Returns(dbSetMock.Object);

        const string projectName = "TimeTracker";

        // Act
        var project = await _projectsProvider.GetProjectByNameAsync(
            projectName,
            _cancellationToken);

        // Assert
        project.Should().BeNull();
    }

    [Fact]
    public async Task GetProjectByNameAsyncWhenProjectWithProvidedNameDoesExistReturnProjectDomainModel()
    {
        // Arrange
        const string projectName = "TimeTracker";

        var dbSetMock = Enumerable.Repeat(new Project { Name = projectName }, 1)
            .ToArray()
            .BuildMockDbSet();

        _dbContextMock.Setup(x => x.Projects)
            .Returns(dbSetMock.Object);

        // Act
        var project = await _projectsProvider.GetProjectByNameAsync(
            projectName,
            _cancellationToken);

        // Assert
        project.Should().NotBeNull();
        project.Should().BeOfType<TimeTrack.Domain.Project>();
    }
}
