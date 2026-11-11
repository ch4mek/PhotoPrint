using MockQueryable.Moq;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UnitTests.Projects;

public class GetProjectsByUserIdRequestTests
{
    private readonly Mock<ITimeTrackDbContext> _timeTrackDbContextMock = new();
    private readonly Mock<IClockService> _clockService = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly GetProjectsByUserIdRequest _handler;

    public GetProjectsByUserIdRequestTests()
    {
        _handler = new GetProjectsByUserIdRequest(
            _timeTrackDbContextMock.Object,
            _clockService.Object);
    }

    [Fact]
    public async Task HandleWhenUserDoesNotHaveProjectsShouldReturnEmptyList()
    {
        // Arrange
        var userId = 1;

        SetupProjects([]);

        // Act
        var result = await _handler.Handle(
            new GetProjectsByUserIdRequest.Query(userId),
            _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleWhenUserTryToGetHisProjectsShouldReturnListOfProjects()
    {
        // Arrange
        var requestedUserId = 1;
        var date = DateTime.UtcNow;

        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                ClientId = 1,
                ManagerUserId = null,
                Color = "#000000",
                CreatedById = 5,
                IsActive = true,
                IsBillable = true,
                IsPublic = false,
                Name = "Test project",
                ProjectUsers =
                [
                    new() {
                        Id = 1,
                        UserId = requestedUserId,
                        User = new() {
                            UserId = requestedUserId,
                            FirstName = "FirstName",
                            LastName = "LastName",Email = "test",
                            DepartmentId = "Administration"
                        },
                        UserOnProjectFrom = DateOnly.FromDateTime(date.AddDays(-10))
                    }
                ],
                TechStack = null,
                Industry = null,
                CloudServices = null,
                Architecture = null
            }
        };

        SetupTime(date);
        SetupProjects(projects);

        var mappedProjects = new List<ProjectWithUsersDto>
        {
            new()
            {
                Id = 1,
                ClientId = 1,
                ManagerId = null,
                Color = "#000000",
                IsActive = true,
                IsBillable = true,
                IsPublic = false,
                Name = "Test project",
                ManagerFullName = null,
                ProjectUsers = new List<ProjectUserInfoDto>
                {
                    new() {
                        DepartmentId = null,
                        FirstName = null,
                        Id = 1,
                        LastName = null,
                    }
                }
            }
        };

        // Act
        var result = await _handler.Handle(
            new GetProjectsByUserIdRequest.Query(requestedUserId),
            _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(mappedProjects);
    }

    private void SetupTime(DateTime date)
    {
        _clockService
            .Setup(mock => mock.UtcNow)
            .Returns(date);
    }

    private void SetupProjects(List<Project> projects)
    {
        _timeTrackDbContextMock
            .Setup(mock => mock.Projects)
            .Returns(projects.BuildMockDbSet().Object);
    }
}
