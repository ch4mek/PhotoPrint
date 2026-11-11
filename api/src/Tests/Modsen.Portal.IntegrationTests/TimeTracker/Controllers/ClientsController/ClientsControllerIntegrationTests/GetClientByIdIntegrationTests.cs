using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.ClientsController.ClientsControllerIntegrationTests;

[Collection("TimeTrackerCollection")]
public class GetClientByIdIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetClientByIdIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetClientByIdShouldReturnExpectedModel()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsCto();
        var clientId = _factory.ModsenClientId!.Value;

        // Act
        var client = await httpClient.GetClientAsync(clientId);

        // Assert
        client.Should().NotBeNull();
        client.Name.Should().Be("Modsen");
    }

    [Fact]
    public async Task GetClientByIdWhenAuthorizedAsEmployeeReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsEmployee();
        var clientId = _factory.ModsenClientId!.Value;

        // Act
        var response = await client.GetClientAsHttpMessageAsync(clientId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetClientByIdWhenAuthorizedAsHeadReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();

        const int clientId = int.MaxValue;

        // Act
        var response = await client.GetClientAsHttpMessageAsync(clientId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetClientByIdAuthedAsGroupManagerWhenUserFromGroupIsAssignedToClientsProjectReturnsClient()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsCeo();

        var createdUserFromGroup = await httpClient.CreateEmployee(resourceGroupId: 1);
        var userId = createdUserFromGroup!.Id;

        var testClient = await httpClient.CreateClientAsync();
        var clientId = testClient!.Id;

        var createdProject = await httpClient.CreateProject(
            clientId: clientId,
            isBillable: false,
            isPublic: false);
        var projectId = createdProject!.Id;

        await httpClient.CreateProjectAssignment(userId: userId, projectId: projectId);

        await httpClient.AuthAs(EmailConstants.GroupManagerEmail);

        // Act
        var client = await httpClient.GetClientAsync(clientId);

        // Assert
        client.Should().NotBeNull();
        client.Name.Should().Be(testClient.Name);
    }

    [Fact]
    public async Task GetClientByIdAuthedAsGroupManagerWhenClientOfUserFromOtherGroupReturnsNotFound()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsCeo();

        var createdUserNotFromGroup = await httpClient.CreateEmployee(
            departmentId: "Java",
            resourceGroupId: 2);
        var userId = createdUserNotFromGroup!.Id;

        var testClient = await httpClient.CreateClientAsync();
        var clientId = testClient!.Id;

        var createdProject = await httpClient.CreateProject(
            clientId: clientId,
            isBillable: false,
            isPublic: false);
        var projectId = createdProject!.Id;

        await httpClient.CreateProjectAssignment(userId: userId, projectId: projectId);

        await httpClient.AuthAs(EmailConstants.GroupManagerEmail);

        // Act
        var response = await httpClient.GetClientAsHttpMessageAsync(clientId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
