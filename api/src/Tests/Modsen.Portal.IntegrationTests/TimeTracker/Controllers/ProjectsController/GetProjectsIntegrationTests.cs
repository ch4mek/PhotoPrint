using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.ProjectsController;

[Collection("TimeTrackerCollection")]
public class GetProjectsIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetProjectsIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Theory]
    [InlineData("")]
    [InlineData("?")]
    [InlineData("?top=50")]
    [InlineData("?skip=0")]
    [InlineData("?top=0&skip=50")]
    [InlineData("?top=50&skip=-1")]
    [InlineData("?top=50&skip=15")]
    [InlineData("?top=26&skip=50")]
    [InlineData("?top=50&skip=100&select=id,Name")]
    [InlineData("?top=50&skip=0&apply=groupby((position), aggregate(id with count as Count))")]
    public async Task GetProjectsAsyncWhenRequestIsInvalidResponseWithBadRequestResult(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetProjectsWithRawSearchAsHttpMessageAsync(query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("?top=10&skip=0")]
    [InlineData("?top=10&skip=50")]
    [InlineData("?top=50&skip=50&filter=contains(Name,'Modsen')")]
    [InlineData("?top=50&skip=50&orderby=Id")]
    [InlineData("?top=50&skip=50&filter=contains(Name,'Modsen')&orderby=Id")]
    public async Task GetProjectsAsyncWhenRequestIsValidResponseWithOk(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetProjectsWithRawSearchAsHttpMessageAsync(query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData(true, 60)]
    [InlineData(false, 0)]
    public async Task GetProjectsAsyncShouldReturnExpectedItems(bool isProjectBillable, int expectedHours)
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var userId = _factory.EmployeeUserId!.Value;

        var createdClient = await client.CreateClientAsync();
        var clientId = createdClient!.Id;

        var project = await client.CreateProject(clientId: clientId,
            isBillable: isProjectBillable);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId,
            isTimeBillable: isProjectBillable);

        var projectUser = await client.GetUserAsync(userId);

        await client.AuthAsEmployee();

        const int trackedTime = 60;
        await client.TrackFactAsync(projectId: projectId, trackedInMinutes: trackedTime);

        await client.AuthAsCeo();

        var expectedResult = new List<ProjectWithUserAndTimeInfoOutput>
        {
            new()
            {
                Id = project.Id,
                Name = project.Name,
                Color = project.Color,
                ClientId = project.ClientId,
                ClientName = createdClient.Name,
                FactTrackedTime = trackedTime,
                BillableTrackedTime = expectedHours,
                IsPublic = project.IsPublic,
                IsBillable = project.IsBillable,
                IsActive = project.IsActive,
                ProjectUsers =
                [
                    new ProjectUserInfoOutput
                    {
                        UserId = _factory.EmployeeUserId!.Value,
                        FirstName = projectUser!.FirstName,
                        LastName = projectUser.LastName,
                        DepartmentId = projectUser.DepartmentId
                    }
                ]
            }
        };

        var filter = $"Name eq '{project.Name}'";
        const int top = 1;

        // Act
        var response = await client.GetProjectsWithSearchAsync(
            filter: filter,
            top: top);

        // Assert
        response.Items.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task GetProjectsAsyncWhenInactiveProjectHasTrackedTimeShouldReturnProjectWithCorrectTrackedTime()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createdClient = await client.CreateClientAsync();
        var clientId = createdClient!.Id;

        var project = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true);
        var projectId = project!.Id;

        await client.AuthAsEmployee();
        const int trackedTime = 60;
        await client.TrackFactAsync(projectId: projectId, trackedInMinutes: trackedTime);

        await client.SetProjectInactiveAsHttpMessageAsync(projectId);

        await client.AuthAsCeo();
        var filter = $"Name eq '{project.Name}'";

        // Act
        var response = await client.GetProjectsWithSearchAsync(
            filter: filter,
            top: 1);

        // Assert
        response.Items.Should().HaveCount(1);
        response.Items.Single().FactTrackedTime.Should().Be(trackedTime);
    }

    [Fact]
    public async Task GetProjectsAsyncWithPostMethodShouldReturnExpectedItems()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var userId = _factory.EmployeeUserId!.Value;

        var createdClient = await client.CreateClientAsync();
        var clientId = createdClient!.Id;

        var project = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: false);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId, isTimeBillable: false);

        await client.AuthAsEmployee();
        var projectUser = await client.GetCurrentUser();

        const int trackedTime = 60;
        await client.TrackFactAsync(projectId: projectId, trackedInMinutes: trackedTime);

        await client.AuthAsCeo();

        var expectedResult = new List<ProjectWithUserAndTimeInfoOutput>
        {
            new()
            {
                Id = project.Id,
                Name = project.Name,
                Color = project.Color,
                ClientId = project.ClientId,
                ClientName = createdClient.Name,
                FactTrackedTime = trackedTime,
                BillableTrackedTime = 0,
                IsPublic = project.IsPublic,
                IsBillable = project.IsBillable,
                IsActive = project.IsActive,
                ProjectUsers =
                [
                    new ProjectUserInfoOutput
                    {
                        UserId = userId,
                        FirstName = projectUser.FirstName,
                        LastName = projectUser.LastName,
                        DepartmentId = projectUser.DepartmentId
                    }
                ]
            }
        };

        var filter = $"filter=Name eq '{project.Name}'";

        // Act
        var response = await client.GetProjectsWithSearchAsPostMethodAsync(filter: filter);

        // Assert
        response.Items.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task GetProjectsAsyncWhenAuthedAsHeadOfDepartmentAndTryToGetProjectShouldReturnProjectWithUsersOnlyFromSameDepartment()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var headId = _factory.HeadOfDepartmentUserId!.Value;

        var userFromMobileDepartment = await client.CreateEmployee(departmentId: "Mobile");
        var employeeMobileId = userFromMobileDepartment!.Id;

        var head = await client.GetUserAsync(headId);

        var userFromHeadDepartment = await client.CreateEmployee(departmentId: head!.DepartmentId!);
        var employeeFromHeadDepartmentId = userFromHeadDepartment!.Id;

        var createdClient = await client.CreateClientAsync();
        var clientId = createdClient!.Id;

        var project = await client.CreateProject(clientId: clientId, isBillable: false);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: employeeMobileId, projectId: projectId);
        await client.CreateProjectAssignment(userId: employeeFromHeadDepartmentId, projectId: projectId);

        await client.AuthAs(head.Email);
        var filter = $"id eq {project.Id}";

        // Act
        var response = await client.GetProjectsWithSearchAsync(
            filter: filter,
            top: 1);

        // Assert
        response.Items.Should().HaveCount(1);
    }
}
