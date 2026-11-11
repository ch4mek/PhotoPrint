using MockQueryable.Moq;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Providers;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.Providers;

namespace Modsen.Portal.TimeTrack.UnitTests.DataAccess;

public class ProjectUsersProviderTests
{
    private readonly Mock<ITimeTrackDbContext> _dbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly IProjectUsersProvider _projectUsersProvider;

    public ProjectUsersProviderTests()
    {
        _projectUsersProvider = new ProjectUsersProvider(_dbContextMock.Object);
    }

    [Fact]
    public async Task GetProjectUsersByProjectIdAsyncWhenUserWithProvidedIdDoesNotExistReturnEmptyList()
    {
        // Arrange
        var dbSetMock = Array.Empty<ProjectUser>().BuildMockDbSet();

        _dbContextMock.Setup(x => x.ProjectUsers)
            .Returns(dbSetMock.Object);

        const int projectId = 645;
        var userIds = new List<int> { 1, 2, 3 };

        // Act
        var project = await _projectUsersProvider.GetProjectUsersByProjectIdAsync(
            projectId,
            userIds,
            _cancellationToken);

        // Assert
        project.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProjectUsersByProjectIdAsyncWhenUserWithProvidedIdsExistsReturnListWithProjectUser()
    {
        // Arrange
        const int projectId = 645;
        var userIds = new List<int> { 5 };

        var dbSetMock = Enumerable
            .Repeat(
                new ProjectUser
                {
                    UserId = userIds[0],
                    ProjectId = projectId,
                    User = new TimeTrackUser
                    {
                        UserId = userIds[0], FirstName = "test", LastName = "test", Email = "test"
                    }
                }, 1)
            .ToArray()
            .BuildMockDbSet();

        _dbContextMock.Setup(x => x.ProjectUsers)
            .Returns(dbSetMock.Object);

        // Act
        var project = await _projectUsersProvider.GetProjectUsersByProjectIdAsync(
            projectId,
            userIds,
            _cancellationToken);

        // Assert
        project.Should().NotBeEmpty();
        project.Should().BeOfType<List<TimeTrack.Domain.ProjectUser>>();
    }

    [Fact]
    public async Task GetProjectUsersByProjectIdsAsyncWhenUserWithProvidedIdDoesNotExistReturnEmptyList()
    {
        // Arrange
        var dbSetMock = Array.Empty<ProjectUser>().BuildMockDbSet();

        _dbContextMock.Setup(x => x.ProjectUsers)
            .Returns(dbSetMock.Object);

        var projectIds = new List<int> { 1, 2, 3 };

        // Act
        var project = await _projectUsersProvider.GetProjectUsersByProjectIdsAsync(
            projectIds,
            _cancellationToken);

        // Assert
        project.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProjectUsersByProjectIdsAsyncWhenUserWithProvidedIdDoesExistReturnListWithProjectUser()
    {
        // Arrange
        var projectIds = new List<int> { 5 };

        var dbSetMock = Enumerable
            .Repeat(
                new ProjectUser
                {
                    ProjectId = projectIds[0],
                    User = new TimeTrackUser { FirstName = "test", LastName = "test", Email = "test" }
                }, 1)
            .ToArray()
            .BuildMockDbSet();

        _dbContextMock.Setup(x => x.ProjectUsers)
            .Returns(dbSetMock.Object);

        // Act
        var project = await _projectUsersProvider.GetProjectUsersByProjectIdsAsync(
            projectIds,
            _cancellationToken);

        // Assert
        project.Should().NotBeEmpty();
        project.Should().BeOfType<List<TimeTrack.Domain.ProjectUser>>();
    }
}
