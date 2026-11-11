using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
namespace Modsen.Portal.IntegrationTests.Identity.Controllers.PermissionAssignmentsController;

[Collection("GeneralCollection")]
public class RevokeUserPermissionIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public RevokeUserPermissionIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task RevokeUserPermissionWhenPermissionCanBeRevokedShouldReturnRemovedAssignment()
    {
        // Arrange
        using var client = _factory.CreateClient();

        await client.AuthAsCeo();

        var user = await client.CreateEmployee();
        var userId = user.Id;

        foreach (var projectId in new[] { _factory.IdleProjectId!.Value, _factory.ModsenProjectId!.Value })
        {
            await client.CreateProjectAssignment(userId: userId, projectId: projectId);
        }

        var assignments = await client.UpdateUserPermissionAssignments(userId: userId,
            resources: new Dictionary<string, string>
            {
                { $"{_factory.IdleProjectId!.Value}", "Test1" },
                { $"{_factory.ModsenProjectId!.Value}", "Test2" }
            });

        // Act
        var deletedAssignment = await client.DeletePermissionAssignment(assignments!.Last().AssignmentId);

        // Assert
        deletedAssignment.Should().BeEquivalentTo(assignments!.Last());
    }

    [Fact]
    public async Task RevokeUserPermissionWhenPermissionAssignmentDoesNotExistShouldResponseWithNotFoundResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const int nonExistingAssignmentId = 9999;

        // Act
        var response = await client.DeletePermissionAssignmentAsHttpMessage(nonExistingAssignmentId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RevokeUserPermissionWhenHeadOfDepartmentRevokesPermissionOfUserFromAnotherDepartmentShouldResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const string departmentId = "Ruby";

        var user = await client.CreateEmployee(departmentId: departmentId);
        var userId = user.Id;

        foreach (var projectId in new[] { _factory.IdleProjectId!.Value, _factory.ModsenProjectId!.Value })
        {
            await client.CreateProjectAssignment(userId: userId, projectId: projectId);
        }

        var assignments = await client.UpdateUserPermissionAssignments(
            userId: userId,
            resources: new Dictionary<string, string>
            {
                { $"{_factory.IdleProjectId!.Value}", "Test1" },
                { $"{_factory.ModsenProjectId!.Value}", "Test2" }
            });

        // Act
        await client.AuthAsHeadOfDepartment();

        var response = await client.DeletePermissionAssignmentAsHttpMessage(assignments!.First().AssignmentId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RevokeUserPermissionWhenGroupManagerRevokesPermissionOfUserFromAnotherResourceGroupShouldResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const string departmentId = "Java";
        const int resourceGroupId = 2;

        var user = await client.CreateEmployee(resourceGroupId: resourceGroupId, departmentId: departmentId);
        var userId = user.Id;

        foreach (var projectId in new[] { _factory.IdleProjectId!.Value, _factory.ModsenProjectId!.Value })
        {
            await client.CreateProjectAssignment(userId: userId, projectId: projectId);
        }

        var assignments = await client.UpdateUserPermissionAssignments(
            userId: userId,
            resources: new Dictionary<string, string>
            {
                { $"{_factory.IdleProjectId!.Value}", "Test1" },
                { $"{_factory.ModsenProjectId!.Value}", "Test2" }
            });

        await client.AuthAs(EmailConstants.GroupManagerEmail);

        // Act
        var response = await client.DeletePermissionAssignmentAsHttpMessage(assignments!.First().AssignmentId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
