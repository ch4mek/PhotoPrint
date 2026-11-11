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
public class GetUserPermissionResourcesIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetUserPermissionResourcesIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetUserPermissionResourcesWhenRequestIsValidShouldResponseWithUserPermissionResources()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var clientId = _factory.ModsenClientId.GetValueOrDefault();

        var user = await client.CreateEmployee();

        var privateProject = await client.CreateProject(clientId: clientId,
            isPublic: false,
            isBillable: true);

        await client.CreateProjectAssignment(userId: user.Id, projectId: privateProject.Id);

        var publicProject = await client.CreateProject(clientId: clientId,
            isPublic: true,
            isBillable: false);

        await client.UpdateUserPermissionAssignments(
            userId: user.Id,
            resources: new Dictionary<string, string>
            {
                { privateProject.Id.ToString(CultureInfo.InvariantCulture), privateProject.Name },
                { publicProject.Id.ToString(CultureInfo.InvariantCulture), publicProject.Name }
            });

        var expectedPermissionResourcesOutput = new UserPermissionResourcesOutput
        {
            UserId = user.Id,
            Resources =
            [
                new ResourceOutput
                {
                    ResourceId = privateProject.Id.ToString(CultureInfo.InvariantCulture),
                    ResourceName = privateProject.Name
                },
                new ResourceOutput
                {
                    ResourceId = publicProject.Id.ToString(CultureInfo.InvariantCulture),
                    ResourceName = publicProject.Name
                }
            ]
        };

        var permissionType = PermissionType.TrackBillableWorkingHours   ;
        var resourceType = PermissionResourceType.Project   ;

        // Act
        var actualPermissionResources = await client.GetPermissionResources(userId: user.Id,
            permissionType: permissionType,
            resourceType: resourceType);

        // Assert
        actualPermissionResources.Should().BeEquivalentTo(expectedPermissionResourcesOutput);
    }

    [Fact]
    public async Task GetUserPermissionResourcesWhenUserDoesNotExistShouldResponseWithNotFoundResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        const int userId = int.MinValue;

        var permissionType = PermissionType.TrackBillableWorkingHours;
        var resourceType = PermissionResourceType.Project;

        // Act
        var response = await client.GetPermissionResourcesAsHttpMessage(userId: userId,
            permissionType: permissionType,
            resourceType: resourceType);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUserPermissionResourcesWhenHeadOfDepartmentTriesToGetResourcesOfUserFromAnotherDepartmentShouldResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        const string departmentId = "Ruby";

        var user = await client.CreateEmployee(departmentId: departmentId);

        var permissionType = PermissionType.TrackBillableWorkingHours;
        var resourceType = PermissionResourceType.Project;

        // Act
        await client.AuthAsHeadOfDepartment();

        var response = await client.GetPermissionResourcesAsHttpMessage(userId: user.Id,
            permissionType: permissionType,
            resourceType: resourceType);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetUserPermissionResourcesWhenGroupManagerTriesToGetResourcesOfUserFromAnotherResourceGroupShouldResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        const string departmentId = "Java";
        const int resourceGroupId = 2;

        var user = await client.CreateEmployee(
            resourceGroupId: resourceGroupId,
            departmentId: departmentId);

        var permissionType = PermissionType.TrackBillableWorkingHours;
        var resourceType = PermissionResourceType.Project;

        // Act
        await client.AuthAs(EmailConstants.GroupManagerEmail);

        var response = await client.GetPermissionResourcesAsHttpMessage(userId: user.Id,
                permissionType: permissionType,
                resourceType: resourceType);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
