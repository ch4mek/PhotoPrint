using MockQueryable.Moq;
using Modsen.Portal.Core.Api.Context;
using Modsen.Portal.Core.Context;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UnitTests.Projects;

public class SortedPersonalProjectsLookupRequestTests
{
    private readonly Mock<ITimeTrackDbContext> _timeTrackDbContextMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new Mock<AuthenticatedUserContext>
        { CallBase = true }.As<IAuthenticatedUserContext>();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly GetSortedPersonalProjectLookupRequest _handler;

    public SortedPersonalProjectsLookupRequestTests()
    {
        _handler = new GetSortedPersonalProjectLookupRequest(
            _timeTrackDbContextMock.Object,
            _authenticatedUserContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenCalledShouldMapAndSortProjectsCorrectly()
    {
        // Arrange
        var request = new GetSortedPersonalProjectLookupRequest.Query(null);
        var userId = 99;

        var projectUser = new ProjectUser
        {
            UserId = userId,
            User = new()
            {
                Email = "test",
                FirstName = "test",
                LastName = "test",
                UserId = userId
            }
        };

        var client = new Client
        {
            Id = 1,
            IsActive = true,
            Name = "Modsen",
        };

        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                ClientId = 1,
                Client = client,
                ManagerUserId = null,
                Color = "#000000",
                CreatedById = 5,
                IsActive = true,
                IsBillable = false,
                IsPublic = true,
                Name = "Arctic",
                ProjectUsers = [projectUser],
                TechStack = null,
                Industry = null,
                CloudServices = null,
                Architecture = null
            },
            new()
            {
                Id = 2,
                ClientId = 1,
                Client = client,
                ManagerUserId = null,
                Color = "#000000",
                CreatedById = 5,
                IsActive = true,
                IsBillable = true,
                IsPublic = false,
                Name = "Bus",
                ProjectUsers = [projectUser],
                TechStack = null,
                Industry = null,
                CloudServices = null,
                Architecture = null
            },
            new()
            {
                Id = 3,
                ClientId = 1,
                Client = client,
                ManagerUserId = null,
                Color = "#000000",
                CreatedById = 5,
                IsActive = true,
                IsBillable = false,
                IsPublic = false,
                Name = "Cookie",
                ProjectUsers = [projectUser],
                TechStack = null,
                Industry = null,
                CloudServices = null,
                Architecture = null
            },
            new()
            {
                Id = 4,
                ClientId = 1,
                Client = client,
                ManagerUserId = null,
                Color = "#000000",
                CreatedById = 5,
                IsActive = true,
                IsBillable = false,
                IsPublic = false,
                Name = "Cake",
                ProjectUsers = [projectUser],
                TechStack = null,
                Industry = null,
                CloudServices = null,
                Architecture = null
            }
        };

        _authenticatedUserContextMock.Setup(x => x.UserId)
            .Returns(userId);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId())
            .Returns(userId);

        SetupProjects(projects);

        var expectedProjects = new List<ProjectSortedLookupDto>
        {
            new()
            {
                Id = 2,
                ClientName = "Modsen",
                CreatedById = 5,
                IsPublic = false,
                Name = "Bus",
                Type = "Private",
                Color = "#000000",
                ProjectUsers = []
            },
            new()
            {
                Id = 4,
                ClientName = "Modsen",
                CreatedById = 5,
                IsPublic = false,
                Name = "Cake",
                Type = "Private",
                Color = "#000000",
                ProjectUsers = []
            },
            new()
            {
                Id = 3,
                ClientName = "Modsen",
                CreatedById = 5,
                IsPublic = false,
                Name = "Cookie",
                Type = "Private",
                Color = "#000000",
                ProjectUsers = []
            },
            new()
            {
                Id = 1,
                ClientName = "Modsen",
                CreatedById = 5,
                IsPublic = true,
                Name = "Arctic",
                Type = "Public",
                Color = "#000000",
                ProjectUsers = []
            }
        };

        // Act
        var result = await _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(
            expectedProjects,
            options => options.WithStrictOrdering());
    }

    private void SetupProjects(List<Project> projects)
    {
        _timeTrackDbContextMock
            .Setup(mock => mock.Projects)
            .Returns(projects.BuildMockDbSet().Object);
    }
}
