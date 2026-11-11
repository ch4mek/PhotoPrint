using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.Dashboard;

[Collection("TimeTrackerCollection")]
public class GetDashboardUserProjectsIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetDashboardUserProjectsIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetUserProjectsAsyncWhenAuthAsEmployeeShouldReturnForbiddenResponse()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsEmployee();
        var ceoId = _factory.CeoUserId!.Value;

        // Act
        var result = await client.GetUserDashboardAsHttpMessageAsync(ceoId);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetUserProjectsAsyncWhenAuthAsCtoWithDepartmentShouldReturnAllUserProjectsFromDepartment()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();
        const string departmentId = "Python";

        var firstUser = await client.CreateEmployee(departmentId: departmentId);
        var firstUserId = firstUser!.Id;
        var secondUser = await client.CreateEmployee(departmentId: departmentId);
        var secondUserId = secondUser!.Id;

        var firstClient = await client.CreateClientAsync();
        var firstClientId = firstClient!.Id;

        var secondClient = await client.CreateClientAsync();
        var secondClientId = secondClient!.Id;

        var firstProject = await client.CreateProject(clientId: firstClientId);
        var firstProjectId = firstProject!.Id;

        var secondProject = await client.CreateProject(clientId: secondClientId, isBillable: false);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: firstUserId, projectId: firstProjectId);
        await client.CreateProjectAssignment(userId: firstUserId, projectId: secondProjectId);
        await client.CreateProjectAssignment(userId: secondUserId, projectId: firstProjectId);

        await client.TrackFactAsync(userId: firstUserId, projectId: firstProjectId);
        await client.TrackFactAsync(userId: firstUserId, projectId: secondProjectId);
        await client.TrackFactAsync(userId: secondUserId, projectId: firstProjectId);

        // Act
        var result = await client.GetUserDashboardAsync(userId: firstUserId,
            departmentId: departmentId,
            page: 1,
            rowsCount: 5);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
        result.FirstOrDefault(u => u.ProjectId == firstProjectId)!.TotalTrackedInMinutes.Should().Be(100);
        result.FirstOrDefault(u => u.ProjectId == secondProjectId)!.TotalTrackedInMinutes.Should().Be(100);
    }

    [Fact]
    public async Task GetUserProjectsAsyncWhenAuthAsHeadOfDepartmentShouldReturnAllUserProjectsFromDepartment()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var firstUser = await client.CreateEmployee();
        var firstUserId = firstUser!.Id;
        var secondUser = await client.CreateEmployee();
        var secondUserId = secondUser!.Id;

        var firstClient = await client.CreateClientAsync();
        var firstClientId = firstClient!.Id;

        var secondClient = await client.CreateClientAsync();
        var secondClientId = secondClient!.Id;

        var firstProject = await client.CreateProject(clientId: firstClientId);
        var firstProjectId = firstProject!.Id;

        var secondProject = await client.CreateProject(clientId: secondClientId, isBillable: false);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: firstUserId, projectId: firstProjectId);
        await client.CreateProjectAssignment(userId: firstUserId, projectId: secondProjectId);
        await client.CreateProjectAssignment(userId: secondUserId, projectId: firstProjectId);

        await client.TrackFactAsync(userId: firstUserId, projectId: firstProjectId);
        await client.TrackFactAsync(userId: firstUserId, projectId: secondProjectId);
        await client.TrackFactAsync(userId: secondUserId, projectId: firstProjectId);

        await client.AuthAsHeadOfDepartment();

        // Act
        var result = await client.GetUserDashboardAsync(userId: firstUserId,
            page: 1, rowsCount: 5);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
        result.FirstOrDefault(u => u.ProjectId == firstProjectId)!.TotalTrackedInMinutes.Should().Be(100);
        result.FirstOrDefault(u => u.ProjectId == secondProjectId)!.TotalTrackedInMinutes.Should().Be(100);
    }

    [Fact]
    public async Task GetUserProjectsAsyncWhenAuthAsHeadOfDepartmentTryGetUserFromOtherDepartmentShouldReturnEmptyList()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var mobileUser = await client.CreateEmployee(departmentId: "Mobile");
        var firstUserId = mobileUser!.Id;
        var secondUser = await client.CreateEmployee(departmentId: "Net");
        var secondUserId = secondUser!.Id;

        var firstClient = await client.CreateClientAsync();
        var firstClientId = firstClient!.Id;

        var secondClient = await client.CreateClientAsync();
        var secondClientId = secondClient!.Id;

        var firstProject = await client.CreateProject(clientId: firstClientId);
        var firstProjectId = firstProject!.Id;

        var secondProject = await client.CreateProject(clientId: secondClientId, isBillable: false);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: firstUserId, projectId: firstProjectId);
        await client.CreateProjectAssignment(userId: firstUserId, projectId: secondProjectId);
        await client.CreateProjectAssignment(userId: secondUserId, projectId: firstProjectId);

        await client.TrackFactAsync(userId: firstUserId, projectId: firstProjectId);
        await client.TrackFactAsync(userId: firstUserId, projectId: secondProjectId);
        await client.TrackFactAsync(userId: secondUserId, projectId: firstProjectId);

        await client.AuthAsHeadOfDepartment();

        // Act
        var result = await client.GetUserDashboardAsync(userId: firstUserId,
            rowsCount: 5, page: 1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUserProjectsAsyncWhenAuthAsGroupMnagerShouldReturnAllUserProjectsFromResorceGroup()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var firstUser = await client.CreateEmployee(resourceGroupId: 1);
        var firstUserId = firstUser!.Id;
        var secondUser = await client.CreateEmployee(resourceGroupId: 1);
        var secondUserId = secondUser!.Id;

        var firstClient = await client.CreateClientAsync();
        var firstClientId = firstClient!.Id;

        var secondClient = await client.CreateClientAsync();
        var secondClientId = secondClient!.Id;

        var firstProject = await client.CreateProject(clientId: firstClientId);
        var firstProjectId = firstProject!.Id;

        var secondProject = await client.CreateProject(clientId: secondClientId, isBillable: false);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: firstUserId, projectId: firstProjectId);
        await client.CreateProjectAssignment(userId: firstUserId, projectId: secondProjectId);
        await client.CreateProjectAssignment(userId: secondUserId, projectId: firstProjectId);

        await client.TrackFactAsync(userId: firstUserId, projectId: firstProjectId);
        await client.TrackFactAsync(userId: firstUserId, projectId: secondProjectId);
        await client.TrackFactAsync(userId: secondUserId, projectId: firstProjectId);

        await client.AuthAs(EmailConstants.GroupManagerEmail);

        // Act
        var result = await client.GetUserDashboardAsync(userId: firstUserId,
            rowsCount: 5, page: 1);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
        result.FirstOrDefault(u => u.ProjectId == firstProjectId)!.TotalTrackedInMinutes.Should().Be(100);
        result.FirstOrDefault(u => u.ProjectId == secondProjectId)!.TotalTrackedInMinutes.Should().Be(100);
    }

    [Fact]
    public async Task GetUserProjectsAsyncWhenAuthAsGroupManagerTryGetUserFromOtherResourceGroupShouldReturnEmptyList()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var firstUser = await client.CreateEmployee(resourceGroupId: 2, departmentId: "Java");
        var firstUserId = firstUser!.Id;
        var secondUser = await client.CreateEmployee(resourceGroupId: 1);
        var secondUserId = secondUser!.Id;

        var firstClient = await client.CreateClientAsync();
        var firstClientId = firstClient!.Id;

        var secondClient = await client.CreateClientAsync();
        var secondClientId = secondClient!.Id;

        var firstProject = await client.CreateProject(clientId: firstClientId);
        var firstProjectId = firstProject!.Id;

        var secondProject = await client.CreateProject(clientId: secondClientId, isBillable: false);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: firstUserId, projectId: firstProjectId);
        await client.CreateProjectAssignment(userId: firstUserId, projectId: secondProjectId);
        await client.CreateProjectAssignment(userId: secondUserId, projectId: firstProjectId);

        await client.TrackFactAsync(userId: firstUserId, projectId: firstProjectId);
        await client.TrackFactAsync(userId: firstUserId, projectId: secondProjectId);
        await client.TrackFactAsync(userId: secondUserId, projectId: firstProjectId);

        await client.AuthAs(EmailConstants.GroupManagerEmail);

        // Act
        var result = await client.GetUserDashboardAsync(userId: firstUserId,
            rowsCount: 5, page: 1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUserProjectsAsyncWhenAuthAsCtoGetUserProjectsToWhichFactAndBillableHoursTrackedShouldReturnProjects()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var user = await client.CreateEmployee(departmentId: "Python");
        var userId = user!.Id;

        var projectClient = await client.CreateClientAsync();
        var clientId = projectClient!.Id;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        await client.TrackFactAsync(userId: userId, projectId: projectId);
        await client.TrackBillableAsync(userId: userId, projectId: projectId);

        // Act
        var result = await client.GetUserDashboardAsync(userId: userId,
            rowsCount: 5, page: 1);

        // Assert
        result.Should().NotBeEmpty();
    }
}
