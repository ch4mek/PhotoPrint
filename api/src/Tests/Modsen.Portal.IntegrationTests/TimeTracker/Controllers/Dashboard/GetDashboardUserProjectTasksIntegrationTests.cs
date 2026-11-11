using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.Dashboard;

[Collection("TimeTrackerCollection")]
public class GetDashboardUserProjectTasksIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetDashboardUserProjectTasksIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetUserProjectAsyncWhenAuthAsEmployeeShouldReturnForbiddenResponse()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsEmployee();
        var ceoId = _factory.CeoUserId!.Value;

        // Act
        var result = await client.GetDashboardUserProjectAsHttpMessageAsync(userId: ceoId,
            projectId: 1);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetUserProjectAsyncWhenAuthAsCtoWithDepartmentShouldReturnAllUserprojectTasksFromDepartment()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();
        const string departmentId = "Python";

        var createdUser = await client.CreateEmployee(departmentId: departmentId);
        var userId = createdUser!.Id;

        var projectClient = await client.CreateClientAsync();
        var clientId = projectClient!.Id;

        var project = await client.CreateProject(clientId: clientId, isBillable: false);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        var firstTask = await client.TrackFactAsync(projectId: projectId, userId: userId);
        var secondTask = await client.TrackFactAsync(projectId: projectId, userId: userId);

        // Act
        var result = await client.GetDashboardUserProjectAsync(
            userId: userId,
            projectId: projectId,
            departmentId: departmentId,
            page: 1,
            rowsCount: 5);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
        result.Any(p => p.TaskName == firstTask.TaskName).Should().BeTrue();
        result.Any(p => p.TaskName == secondTask.TaskName).Should().BeTrue();
    }

    [Fact]
    public async Task GetUserProjectAsyncWhenAuthAsHeadOfDepartmentShouldReturnAllUserProjectTasksFromDepartment()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var createdUser = await client.CreateEmployee();
        var userId = createdUser!.Id;

        var projectClient = await client.CreateClientAsync();
        var clientId = projectClient!.Id;

        var project = await client.CreateProject(clientId: clientId, isBillable: false);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        var firstTask = await client.TrackFactAsync(projectId: projectId, userId: userId);
        var secondTask = await client.TrackFactAsync(projectId: projectId, userId: userId);

        await client.AuthAsHeadOfDepartment();

        // Act
        var result = await client.GetDashboardUserProjectAsync(userId: userId,
            projectId: projectId);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
        result.Any(p => p.TaskName == firstTask.TaskName).Should().BeTrue();
        result.Any(p => p.TaskName == secondTask.TaskName).Should().BeTrue();
    }

    [Fact]
    public async Task GetUserProjectsAsyncWhenAuthAsHeadOfDepartmentTryGetuserTasksFromOtherDepartmentShouldReturnEmptyList()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();
        const string departmentId = "Mobile";

        var createdUser = await client.CreateEmployee(departmentId: departmentId);
        var userId = createdUser!.Id;

        var projectClient = await client.CreateClientAsync();
        var clientId = projectClient!.Id;

        var project = await client.CreateProject(clientId: clientId, isBillable: false);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        await client.TrackFactAsync(projectId: projectId, userId: userId);
        await client.TrackFactAsync(projectId: projectId, userId: userId);

        await client.AuthAsHeadOfDepartment();

        // Act
        var result = await client.GetDashboardUserProjectAsync(userId: userId,
            projectId: projectId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUserProjectAsyncWhenAuthAsGroupManagerShouldReturnAllUserProjectTasksFromRespurceGroup()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var createdUser = await client.CreateEmployee(resourceGroupId: 1);
        var userId = createdUser!.Id;

        var projectClient = await client.CreateClientAsync();
        var clientId = projectClient!.Id;

        var project = await client.CreateProject(clientId: clientId, isBillable: false);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        var firstTask = await client.TrackFactAsync(projectId: projectId, userId: userId);
        var secondTask = await client.TrackFactAsync(projectId: projectId, userId: userId);

        await client.AuthAs(EmailConstants.GroupManagerEmail);

        // Act
        var result = await client.GetDashboardUserProjectAsync(userId: userId,
            projectId: projectId);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
        result.Any(p => p.TaskName == firstTask.TaskName).Should().BeTrue();
        result.Any(p => p.TaskName == secondTask.TaskName).Should().BeTrue();
    }

    [Fact]
    public async Task GetUserProjectsAsyncWhenAuthAsGroupManagerTryGetuserTasksFromOtherResourceGroupShouldReturnEmptyList()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var createdUser = await client.CreateEmployee(resourceGroupId: 2, departmentId: "Java");
        var userId = createdUser!.Id;

        var projectClient = await client.CreateClientAsync();
        var clientId = projectClient!.Id;

        var project = await client.CreateProject(clientId: clientId, isBillable: false);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        await client.TrackFactAsync(projectId: projectId, userId: userId);
        await client.TrackFactAsync(projectId: projectId, userId: userId);

        await client.AuthAs(EmailConstants.GroupManagerEmail);

        // Act
        var result = await client.GetDashboardUserProjectAsync(userId: userId,
            projectId: projectId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
