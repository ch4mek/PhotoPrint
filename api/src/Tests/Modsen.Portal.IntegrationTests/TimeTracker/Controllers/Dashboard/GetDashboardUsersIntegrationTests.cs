using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.IntegrationTests.TestFramework.IntegrationTestFactoryExtensions;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.Dashboard;

[Collection("TimeTrackerCollection")]
public class GetDashboardUsersIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetDashboardUsersIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _factory.CleanTimesheets();
    }

    [Fact]
    public async Task GetUsersAsyncWhenAuthAsEmployeeShouldReturnForbiddenResponse()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var result = await client.GetDashboardUsersAsHttpMessageAsync();

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetUsersAsyncWhenAuthAsCtoWithDepartmentShouldReturnAllUsersFromDepartment()
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
        await client.CreateProjectAssignment(userId: secondUserId, projectId: secondProjectId);

        var timesheets = new List<TimesheetCoreDto>
        {
            await client.TrackFactAsync(userId: firstUserId, projectId: firstProjectId),
            await client.TrackFactAsync(userId: firstUserId, projectId: secondProjectId),
            await client.TrackFactAsync(userId: firstUserId, projectId: secondProjectId)
        };

        // Act
        var result = await client.GetDashboardUsersAsync(departmentId: departmentId,
            page: 1,
            rowsCount: 100);

        // Assert
        result.Should().NotBeNull();
        result.Items.Count.Should().BeGreaterThanOrEqualTo(2);
        result.Items.FirstOrDefault(u => u.UserId == secondUserId).Should().NotBeNull();
        result.Items.FirstOrDefault(u => u.UserId == firstUserId)!.TotalTrackedInMinutes.Should().Be(300);

        await client.DeleteTimesheetsUnderCeoRoleAsync(timesheets);
    }

    [Fact]
    public async Task GetUsersAsyncWhenEmployeeTimesheetHasBillableHoursShouldReturnEmployee()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();
        const string departmentId = "Python";

        var user = await client.CreateEmployee(departmentId: departmentId);
        var userId = user!.Id;

        var projectClient = await client.CreateClientAsync();
        var clientId = projectClient!.Id;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        await client.TrackFactAsync(userId: userId, projectId: projectId);
        var timesheet = await client.TrackBillableAsync(userId: userId, projectId: projectId);

        // Act
        var result = await client.GetDashboardUsersAsync(
            departmentId: departmentId,
            rowsCount: 1000, page: 1);

        // Assert
        result.Should().NotBeNull();
        result.Items.Count(u => u.UserId == userId).Should().Be(1);
        result.Items.FirstOrDefault(u => u.UserId == userId)!.TotalTrackedInMinutes.Should().Be(100);

        await client.DeleteTimesheetsUnderCeoRoleAsync([timesheet]);
    }

    [Fact]
    public async Task GetUsersAsyncWhenAuthAsHeadOfDepartmentShouldReturnAllUsersFromDepartment()
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
        await client.CreateProjectAssignment(userId: secondUserId, projectId: secondProjectId);

        await client.TrackFactAsync(userId: firstUserId, projectId: firstProjectId);
        await client.TrackFactAsync(userId: firstUserId, projectId: secondProjectId);
        await client.TrackFactAsync(userId: secondUserId, projectId: secondProjectId);

        await client.AuthAsHeadOfDepartment();

        // Act
        var result = await client.GetDashboardUsersAsync(rowsCount: 1000, page: 1);

        // Assert
        result.Should().NotBeNull();
        result.Items.Count.Should().BeGreaterThanOrEqualTo(2);
        result.Items.Should().Contain(u => u.UserId == firstUserId);
        result.Items.First(u => u.UserId == firstUserId).TotalTrackedInMinutes.Should().Be(200);
    }

    [Fact]
    public async Task GetUsersAsyncWhenEmployeeTrackedFactHoursShouldReturnUsersProjectsToWhichFactHoursWereTracked()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();
        const string departmentId = "Python";

        var user = await client.CreateEmployee(departmentId: departmentId);
        var userId = user!.Id;

        var projectClient = await client.CreateClientAsync();
        var clientId = projectClient!.Id;

        var firstProject = await client.CreateProject(clientId: clientId);
        var firstProjectId = firstProject!.Id;

        var secondProject = await client.CreateProject(clientId: clientId, isBillable: false);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: firstProjectId);
        await client.CreateProjectAssignment(userId: userId, projectId: secondProjectId);

        var timesheets = new List<TimesheetCoreDto>
        {
            await client.TrackFactAsync(userId: userId, projectId: secondProjectId),
            await client.TrackBillableAsync(userId: userId, projectId: secondProjectId),
            await client.TrackBillableAsync(userId: userId, projectId: firstProjectId),
        };

        // Act
        var result = await client.GetDashboardUsersAsync(
            departmentId: departmentId,
            page: 1,
            rowsCount: 1000);

        // Assert
        result.Items.Where(usr => usr.UserId == userId).FirstOrDefault().CountOfProjects.Should().Be(1);

        await client.DeleteTimesheetsUnderCeoRoleAsync(timesheets);
    }

    [Fact]
    public async Task GetUsersAsyncWhenAuthAsGroupManagerShouldReturnAllUsersFromResourceGroup()
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
        await client.CreateProjectAssignment(userId: secondUserId, projectId: secondProjectId);

        await client.TrackFactAsync(userId: firstUserId, projectId: firstProjectId);
        await client.TrackFactAsync(userId: firstUserId, projectId: secondProjectId);
        await client.TrackFactAsync(userId: secondUserId, projectId: secondProjectId);

        await client.AuthAs(EmailConstants.GroupManagerEmail);

        // Act
        var result = await client.GetDashboardUsersAsync(page: 1, rowsCount: 100);

        // Assert
        result.Should().NotBeNull();
        result.Items.Count.Should().BeGreaterThanOrEqualTo(2);
        result.Items.Should().Contain(u => u.UserId == firstUserId);
        result.Items.First(u => u.UserId == firstUserId).TotalTrackedInMinutes.Should().Be(200);
    }
}
