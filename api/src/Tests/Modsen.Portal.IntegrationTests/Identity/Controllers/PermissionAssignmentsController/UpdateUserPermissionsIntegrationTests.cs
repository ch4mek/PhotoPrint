using System.Globalization;
using System.Net;
using System.Text.Json;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Controllers.Contracts.Output;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
namespace Modsen.Portal.IntegrationTests.Identity.Controllers.PermissionAssignmentsController;

[Collection("GeneralCollection")]
public class UpdateUserPermissionsIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public UpdateUserPermissionsIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task UpdateUserPermissionsWhenUserPermissionsCanBeUpdatedShouldResponseWithUpdatedAssignments()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var clientId = _factory.ModsenClientId.GetValueOrDefault();

        var user = await client.CreateEmployee();
        var userId = user!.Id;

        var privateProject = await client.CreateProject(clientId: clientId,
            isPublic: false,
            isBillable: true);
        var privateProjectId = privateProject!.Id.ToString(CultureInfo.InvariantCulture);

        await client.CreateProjectAssignment(userId: userId, projectId: privateProject.Id);

        var publicProject = await client.CreateProject(clientId: clientId,
            isPublic: true,
            isBillable: false);
        var publicProjectId = publicProject!.Id.ToString(CultureInfo.InvariantCulture);

        var expectedAssignments = new List<PermissionAssignmentOutput>
        {
            new()
            {
                AssignmentId = 0,
                PermissionType = PermissionType.TrackBillableWorkingHours,
                ResourceId = privateProjectId,
                ResourceName = privateProject.Name,
                ResourceType = PermissionResourceType.Project,
                UserId = user.Id
            },
            new()
            {
                AssignmentId = 0,
                PermissionType = PermissionType.TrackBillableWorkingHours,
                ResourceId = publicProjectId,
                ResourceName = publicProject.Name,
                ResourceType = PermissionResourceType.Project,
                UserId = user.Id
            }
        };

        // Act
        var assignments = await client.UpdateUserPermissionAssignments(
            userId: userId,
            resources: new Dictionary<string, string>
            {
                { privateProjectId, privateProject.Name }, { publicProjectId, publicProject.Name }
            });

        // Assert
        assignments.Should().NotBeNull();
        assignments.Should().BeEquivalentTo(expectedAssignments, options =>
            options.Excluding(x => x.AssignmentId));
    }

    [Fact]
    public async Task UpdateUserPermissionsWhenUserDoesNotExistShouldResponseWithNotFoundResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const int nonExistingUserId = int.MaxValue;

        // Act
        var response = await client.UpdateUserPermissionAssignmentsAsHttpMessage(
            userId: nonExistingUserId,
            resources: new Dictionary<string, string> { { "1", "Test1" }, { "2", "Test2" } }, _jsonOptions);

        // Arrange
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateUserPermissionsWhenUserIsNotEmployeeShouldResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const string departmentId = "Ruby";
        var idleProjectId = _factory.IdleProjectId!.Value;
        var modsenProjectId = _factory.ModsenProjectId!.Value;

        var user = await client.CreateEmployee(departmentId: departmentId);
        var userId = user!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: idleProjectId);

        // Act
        var response = await client.UpdateUserPermissionAssignmentsAsHttpMessage(
            userId: userId,
            resources: new Dictionary<string, string>
            {
                { $"{idleProjectId}", "Test1" },
                { $"{modsenProjectId}", "Test2" }
            }, jsonOptions: _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserPermissionsWhenUserHasNoDepartmentShouldResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var sysAdminHeadId = _factory.SysAdminHeadId!.Value;
        var idleProjectId = _factory.IdleProjectId!.Value;
        var modsenProjectId = _factory.ModsenProjectId!.Value;

        // Act
        var response = await client.UpdateUserPermissionAssignmentsAsHttpMessage(
            userId: sysAdminHeadId,
            resources: new Dictionary<string, string>
            {
                { $"{idleProjectId}", "Test1" },
                { $"{modsenProjectId}", "Test2" }
            }, _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserPermissionsWhenHeadOfDepartmentUpdatesPermissionsOfUserFromAnotherDepartmentShouldResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const string departmentId = "Ruby";
        var idleProjectId = _factory.IdleProjectId!.Value;
        var modsenProjectId = _factory.ModsenProjectId!.Value;

        var user = await client.CreateEmployee(departmentId: departmentId);
        var userId = user!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: idleProjectId);

        // Act
        await client.AuthAsHeadOfDepartment();

        var response = await client.UpdateUserPermissionAssignmentsAsHttpMessage(
            userId: userId,
            resources: new Dictionary<string, string>
            {
                { $"{idleProjectId}", "Test1" },
                { $"{modsenProjectId}", "Test2" }
            }, _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserPermissionsWhenResourcesDoNotExistShouldResponseWithNotFoundResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const string departmentId = "Ruby";
        var idleProjectId = _factory.IdleProjectId!.Value;
        var nonExistingResourceId = int.MaxValue.ToString(CultureInfo.InvariantCulture);

        var user = await client.CreateEmployee(departmentId: departmentId);
        var userId = user!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: idleProjectId);

        // Act
        var response = await client.UpdateUserPermissionAssignmentsAsHttpMessage(
            userId: userId,
            resources: new Dictionary<string, string>
            {
                { nonExistingResourceId, "Test" }
            }, _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateUserPermissionsWhenUserDoesNotHaveAccessToSomeProjectsShouldResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        const string departmentId = "Ruby";
        var idleProjectId = _factory.IdleProjectId!.Value;
        var modsenClientId = _factory.ModsenClientId!.Value;

        var user = await client.CreateEmployee(departmentId: departmentId);
        var userId = user!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: idleProjectId);

        var project = await client.CreateProject(clientId: modsenClientId,
            isPublic: false,
            isBillable: true);

        // Act
        var response = await client.UpdateUserPermissionAssignmentsAsHttpMessage(
            userId: userId,
            resources: new Dictionary<string, string>
            {
                { $"{idleProjectId}", "Test1" },
                { project!.Id.ToString(CultureInfo.InvariantCulture), project.Name }
            }, _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserPermissionsWhenGroupManagerUpdatesPermissionsOfUserFromAnotherResourceGroupShouldResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const string departmentId = "Java";
        const int resourceGroupId = 2;
        var idleProjectId = _factory.IdleProjectId!.Value;

        var user = await client.CreateEmployee(resourceGroupId: resourceGroupId, departmentId: departmentId);
        var userId = user!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: idleProjectId);

        // Act
        await client.AuthAs(EmailConstants.GroupManagerEmail);

        var response = await client.UpdateUserPermissionAssignmentsAsHttpMessage(userId: userId,
            resources: new Dictionary<string, string>
            {
                { $"{idleProjectId}", "Test1" },
                { $"{_factory.ModsenProjectId!.Value}", "Test2" }
            }, _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
