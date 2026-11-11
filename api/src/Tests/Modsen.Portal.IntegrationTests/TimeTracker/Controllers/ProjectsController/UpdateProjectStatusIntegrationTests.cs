using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.ProjectsController;

[Collection("TimeTrackerCollection")]
public class UpdateProjectStatusIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    public UpdateProjectStatusIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task UpdateProjectStatusAsyncWhenAuthedAsEmployeeShouldResponseWithForbiddenResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var project = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true);
        var projectId = project!.Id;

        await client.AuthAsEmployee();

        // Act
        var response = await client.SetProjectInactiveAsHttpMessageAsync(projectId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateProjectStatusAsyncWhenProjectDoesNotExistShouldResponseWithNotFoundResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const int projectId = int.MaxValue;

        // Act
        var response = await client.SetProjectInactiveAsHttpMessageAsync(projectId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateProjectStatusAsyncWhenProjectHasSameStatusShouldResponseWithBadRequestResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var project = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true);
        var projectId = project!.Id;

        // Act
        var response = await client.SetProjectActiveAsHttpMessageAsync(projectId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateProjectStatusAsyncWhenDeactivatingProjectWithAssignedUsersShouldResponseWithBadRequestResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var userId = _factory.EmployeeUserId!.Value;
        var headId = _factory.HeadOfDepartmentUserId!.Value;

        var project = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);
        await client.CreateProjectAssignment(userId: headId, projectId: projectId);

        // Act
        var response = await client.SetProjectInactiveAsHttpMessageAsync(projectId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateProjectStatusAsyncWhenHeadOfDeprtmentTriesToReactivateProjectShouldResponseWithForbiddenResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var project = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true);
        var projectId = project!.Id;

        await client.SetProjectInactiveAsHttpMessageAsync(projectId);

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.SetProjectActiveAsHttpMessageAsync(projectId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateProjectStatusAsyncWhenReactivatingProjectShouldResponseWithOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var project = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true);
        var projectId = project!.Id;

        await client.SetProjectInactiveAsHttpMessageAsync(projectId);

        // Act
        var response = await client.SetProjectActiveAsync(projectId);

        // Assert
        response.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateProjectStatusAsyncWhenDeactivatingProjectWithoutAssignedUsersShouldResponseWithOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();
        var clientId = _factory.ModsenClientId!.Value;

        var project = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true);
        var projectId = project!.Id;

        // Act
        var response = await client.SetProjectInactiveAsync(projectId);

        // Assert
        response.Should().BeFalse();
    }
}
