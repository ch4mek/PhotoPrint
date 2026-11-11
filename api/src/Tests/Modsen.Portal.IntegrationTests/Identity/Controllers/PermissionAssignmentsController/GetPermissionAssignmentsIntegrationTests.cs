using System.Globalization;
using System.Text.Json;
using Modsen.Portal.Identity.Controllers.Contracts.Output;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
namespace Modsen.Portal.IntegrationTests.Identity.Controllers.PermissionAssignmentsController;

[Collection("GeneralCollection")]
public class GetPermissionAssignmentsIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetPermissionAssignmentsIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetProjectPermissionAssignmentsWhenRequestIsValidShouldResponseWithPermissionAssignments()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var clientId = _factory.ModsenClientId.GetValueOrDefault();
        const int pageSize = 100;

        var employee = await client.CreateEmployee();

        var privateProject = await client.CreateProject(clientId: clientId,
            isBillable: true,
            isPublic: false);

        await client.CreateProjectAssignment(
            userId: employee.Id,
            projectId: privateProject.Id,
            isTimeBillable: true);

        var publicProject = await client.CreateProject(clientId: clientId,
            isBillable: false,
            isPublic: true);

        var assignments = await client.UpdateUserPermissionAssignments(
            userId: employee.Id,
            resources: new Dictionary<string, string>
            {
                { privateProject.Id.ToString(CultureInfo.InvariantCulture), privateProject.Name },
                { publicProject!.Id.ToString(CultureInfo.InvariantCulture), publicProject.Name }
            });

        var firstAssignment = assignments!.First(assignment => assignment.ResourceId == privateProject.Id.ToString());
        var secondAssignment = assignments!.First(assignment => assignment.ResourceId == publicProject.Id.ToString());

        var expectedAssignments = new List<ProjectPermissionAssignmentOutput>
        {
            new()
            {
                AssignmentId = firstAssignment.AssignmentId,
                UserId = employee.Id,
                ProjectId = privateProject.Id.ToString(CultureInfo.InvariantCulture),
                UserFullName = $"{employee.LastName} {employee.FirstName}",
                ProjectName = firstAssignment.ResourceName,
                UserColor = employee.Color
            },
            new()
            {
                AssignmentId = secondAssignment.AssignmentId,
                UserId = employee.Id,
                ProjectId = publicProject.Id.ToString(CultureInfo.InvariantCulture),
                UserFullName = $"{employee.LastName} {employee.FirstName}",
                ProjectName = secondAssignment.ResourceName,
                UserColor = employee.Color
            }
        };


        // Act
        var actualAssignments = await client.GetPermissionAssignments(_jsonOptions, top: pageSize);

        // Assert
        actualAssignments!.Items.Should().ContainEquivalentOf(expectedAssignments[0]);
        actualAssignments.Items.Should().ContainEquivalentOf(expectedAssignments[1]);
        actualAssignments.Total.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetProjectPermissionAssignmentsWhenSearchingByUserNameShouldResponseWithPermissionAssignmentsWithAccordingUserName()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const int pageSize = 10;
        var clientId = _factory.ModsenClientId.GetValueOrDefault();

        var employee = await client.CreateEmployee();
        var searchEmployee = await client.CreateEmployee();

        var privateProject = await client.CreateProject(clientId: clientId,
            isPublic: false,
            isBillable: true);

        await client.CreateProjectAssignment(userId: employee.Id,
            projectId: privateProject.Id);

        await client.CreateProjectAssignment(userId: searchEmployee.Id,
            projectId: privateProject.Id);

        await client.UpdateUserPermissionAssignments(userId: employee.Id,
            resources: new Dictionary<string, string>
            {
                { privateProject.Id.ToString(CultureInfo.InvariantCulture), privateProject.Name }
            });

        var searchAssignments = await client.UpdateUserPermissionAssignments(userId: searchEmployee.Id,
            resources: new Dictionary<string, string>
            {
                { privateProject.Id.ToString(CultureInfo.InvariantCulture), privateProject.Name }
            });

        var expectedAssignments = new List<ProjectPermissionAssignmentOutput>
        {
            new()
            {
                AssignmentId = searchAssignments![0].AssignmentId,
                UserId = searchEmployee.Id,
                ProjectId = privateProject.Id.ToString(CultureInfo.InvariantCulture),
                UserFullName = $"{searchEmployee.LastName} {searchEmployee.FirstName}",
                ProjectName = searchAssignments[0].ResourceName,
                UserColor = searchEmployee.Color
            }
        };

        // Act
        var actualAssignments = await client.GetPermissionAssignments(_jsonOptions, filter: $"contains(tolower(userFullName), tolower('{searchEmployee.LastName}'))",
            top: pageSize);

        // Assert
        actualAssignments!.Items.Should().ContainEquivalentOf(expectedAssignments[0]);
        actualAssignments.Total.Should().BeGreaterThanOrEqualTo(1);
    }
}
