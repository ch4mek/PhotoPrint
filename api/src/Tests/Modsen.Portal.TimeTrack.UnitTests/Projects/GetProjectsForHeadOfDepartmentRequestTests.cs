using Microsoft.AspNetCore.OData.Query;
using MockQueryable.Moq;
using Modsen.Portal.Core.Api.Context;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.OData;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.OData;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

namespace Modsen.Portal.TimeTrack.UnitTests.Projects;

public class GetProjectsForHeadOfDepartmentRequestTests
{
    private readonly Mock<ITimeTrackDbContext> _timeTrackDbContextMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new Mock<AuthenticatedUserContext>
        { CallBase = true }.As<IAuthenticatedUserContext>();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly GetProjectsRequest _queryHandler;

    public GetProjectsForHeadOfDepartmentRequestTests()
    {
        _queryHandler = new GetProjectsRequest(
            _timeTrackDbContextMock.Object,
            _authenticatedUserContextMock.Object);
    }

    [Theory]
    [InlineData(null)]
    public async Task HandleShouldCallGetPageableProjectsDbQuery(string? departmentId)
    {
        // Arrange
        const int projectId = 64;
        const int userId = 1;

        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "50",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
            [nameof(ODataRawQueryOptions.Filter)] = $"id eq {projectId}"
        };

        var queryOptions = ODataQueryOptionsMock.BuildMock<OdpProject>(queryParameters);
        var request = new GetProjectsRequest.Query(queryOptions);

        var projects = new List<Project>
        {
            new()
            {
                Id = projectId,
                Name = "Idle",
                IsPublic = false,
                IsBillable = false,
                IsActive = true,
                Color = "#00ff83",
                ClientId = 1,
                ProjectUsers = [],
                Client = new Client
                {
                    Id = 1,
                    Name = "client"
                },
            }
        };

        _authenticatedUserContextMock.Setup(x => x.DepartmentId)
            .Returns(departmentId);

        _authenticatedUserContextMock.Setup(x => x.UserId)
            .Returns(userId);

        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId())
            .Returns(userId);

        SetupProjects(projects);
        SetupProjectUsers([]);
        SetupUsers([]);

        // Act
        var result = await _queryHandler.Handle(
            request,
            _cancellationToken);

        // Assert
        result.Items.Count.Should().Be(1);
    }

    private void SetupProjects(List<Project> projects)
    {
        _timeTrackDbContextMock
            .Setup(mock => mock.Projects)
            .Returns(projects.BuildMockDbSet().Object);
    }

    private void SetupProjectUsers(List<ProjectUser> projectUsers)
    {
        _timeTrackDbContextMock
            .Setup(mock => mock.ProjectUsers)
            .Returns(projectUsers.BuildMockDbSet().Object);
    }

    private void SetupUsers(List<TimeTrackUser> users)
    {
        _timeTrackDbContextMock
            .Setup(mock => mock.TimeTrackUsers)
            .Returns(users.BuildMockDbSet().Object);
    }
}
