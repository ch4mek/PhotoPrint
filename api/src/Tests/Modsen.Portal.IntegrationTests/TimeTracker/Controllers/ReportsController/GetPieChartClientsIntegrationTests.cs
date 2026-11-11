using System.Globalization;
using System.Net;
using System.Text.Json;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.IntegrationTests.TestFramework.IntegrationTestFactoryExtensions;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.ReportsController;

[Collection("TimeTrackerCollection")]
public class GetPieChartClientsIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetPieChartClientsIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _factory.CleanTimesheets();
    }

    [Fact]
    public async Task GetPieChartClientsWhenAuthAsEmployeeShouldReturnForbiddenResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var action = await client.GetPieChartClientsAsHttpMessageAsync();

        // Assert
        action.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetPieChartClientsWhenAuthAsHeadShouldReturnHeadDepartmentClientsTimeForTheSpecifiedPeriod()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var createdUser = await client.CreateEmployee();
        var userId = createdUser!.Id;

        var projectClient = await client.CreateClientAsync();
        var clientId = projectClient!.Id;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        await client.AuthAs(createdUser.Email);

        var timesheets = new List<TimesheetCoreDto>
        {
            await client.TrackFactAsync(projectId: projectId, trackedInMinutes: 150, date: date),
            await client.TrackFactAsync(projectId: projectId, trackedInMinutes: 100, date: date)
        };

        // Act
        await client.AuthAsHeadOfDepartment();
        var result = await client.GetPieChartClientsAsync(year: date.Year, month: date.Month,
            trackedTimeType: TrackedTimeType.Billable);

        // Assert
        result.Should().NotBeNull();
        result.Should().ContainSingle();
        result[0].Name.Should().Be(projectClient.Name);
        result[0].Color.Should().Be(projectClient.Color);
        result[0].TotalTimeInMinutes.Should().Be(250);

        await client.DeleteTimesheetsUnderCeoRoleAsync(timesheets);
    }

    [Fact]
    public async Task GetPieChartClientsWhenAuthAsCeoShouldReturnClientsTimeForTheSpecifiedPeriod()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1).Date);
        var userId = _factory.EmployeeUserId!.Value;
        var ceoId = _factory.CeoUserId!.Value;

        var firstClient = await client.CreateClientAsync();
        var firstClientId = firstClient!.Id;

        var secondClient = await client.CreateClientAsync();
        var secondClientId = secondClient!.Id;

        var firstProject = await client.CreateProject(clientId: firstClientId);
        var firstProjectId = firstProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: firstProjectId);

        var secondProject = await client.CreateProject(clientId: secondClientId);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: secondProjectId);
        await client.CreateProjectAssignment(userId: ceoId, projectId: secondProjectId);

        await client.AuthAsEmployee();
        await client.TrackFactAsync(projectId: firstProjectId, trackedInMinutes: 480, date: date);
        await client.TrackFactAsync(projectId: secondProjectId, trackedInMinutes: 120, date: date);
        await client.TrackFactAsync(projectId: secondProjectId, trackedInMinutes: 150, date: date);

        await client.AuthAsCeo();

        // Act
        var result = await client.GetPieChartClientsAsync(year: date.Year,
            month: date.Month,
            trackedTimeType: TrackedTimeType.Billable);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().BeGreaterThanOrEqualTo(2);

        var seriousClient1 = result.Find(x => x.Name == firstClient.Name);
        seriousClient1.Should().NotBeNull();
        seriousClient1.Color.Should().Be(firstClient.Color);
        seriousClient1.TotalTimeInMinutes.Should().Be(480);

        var seriousClient2 = result.Find(x => x.Name == secondClient.Name);
        seriousClient2.Should().NotBeNull();
        seriousClient2.Color.Should().Be(secondClient.Color);
        seriousClient2.TotalTimeInMinutes.Should().Be(270);
    }

    [Fact]
    public async Task GetPieChartClientsWhenTakeBillableHoursShouldReturnClientsTimeForTheSpecifiedPeriod()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1).Date);
        var userId = _factory.EmployeeUserId!.Value;
        var ceoId = _factory.CeoUserId!.Value;

        var firstClient = await client.CreateClientAsync();
        var firstClientId = firstClient!.Id;

        var secondClient = await client.CreateClientAsync();
        var secondClientId = secondClient!.Id;

        var firstProject = await client.CreateProject(clientId: firstClientId);
        var firstProjectId = firstProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: firstProjectId);

        var secondProject = await client.CreateProject(clientId: secondClientId, isBillable: false);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: secondProjectId);
        await client.CreateProjectAssignment(userId: ceoId, projectId: secondProjectId);

        await client.UpdateUserPermissionAssignments(
            userId: userId,
            resources: new Dictionary<string, string>
            {
                { firstProjectId.ToString(CultureInfo.InvariantCulture), firstProject.Name },
                { secondProjectId.ToString(CultureInfo.InvariantCulture), secondProject.Name }
            });

        await client.AuthAsEmployee();
        await client.TrackFactAsync(projectId: firstProjectId, trackedInMinutes: 100, date: date);
        await client.TrackFactAsync(projectId: secondProjectId, trackedInMinutes: 100, date: date);
        await client.TrackFactAsync(projectId: secondProjectId, trackedInMinutes: 100, date: date);

        await client.TrackBillableAsync(projectId: firstProjectId, trackedInMinutes: 480, date: date);
        await client.TrackBillableAsync(projectId: secondProjectId, trackedInMinutes: 160, date: date);
        await client.TrackBillableAsync(projectId: secondProjectId, trackedInMinutes: 250, date: date);

        await client.AuthAsCeo();

        // Act
        var clients = await client.GetPieChartClientsAsync(year: date.Year,
            month: date.Month,
            trackedTimeType: TrackedTimeType.Billable);

        // Assert
        clients.Should().NotBeNull();
        clients.Count.Should().BeGreaterThanOrEqualTo(2);

        var seriousClient1 = clients.Find(x => x.Name == firstClient.Name);
        seriousClient1.Should().NotBeNull();
        seriousClient1.Color.Should().Be(firstClient.Color);
        seriousClient1.TotalTimeInMinutes.Should().Be(480);

        var seriousClient2 = clients.Find(x => x.Name == secondClient.Name);
        seriousClient2.Should().NotBeNull();
        seriousClient2.Color.Should().Be(secondClient.Color);
        seriousClient2.TotalTimeInMinutes.Should().Be(410);
    }

    [Fact]
    public async Task GetPieChartClientsWhenAuthAsGmShouldReturnGroupManagersClientsTimeForTheSpecifiedPeriod()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var createdUser = await client.CreateEmployee(resourceGroupId: 1);
        var netUserId = createdUser!.Id;

        var createdUserNotInGroup = await client.CreateEmployee(departmentId: "Java", resourceGroupId: 2);
        var javaUserId = createdUserNotInGroup!.Id;

        var projectClient = await client.CreateClientAsync();
        var clientId = projectClient!.Id;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: netUserId, projectId: projectId);
        await client.CreateProjectAssignment(userId: javaUserId, projectId: projectId);

        var timesheets = new List<TimesheetCoreDto>
        {
            await client.TrackFactAsync(projectId: projectId, userId: netUserId, trackedInMinutes: 150, date: date),
            await client.TrackFactAsync(projectId: projectId, userId: netUserId, trackedInMinutes: 100, date: date),
            await client.TrackFactAsync(projectId: projectId, userId: javaUserId, trackedInMinutes: 100, date: date)
        };

        await client.TrackBillableAsync(projectId: projectId, userId: netUserId, trackedInMinutes: 50, date: date);
        await client.TrackBillableAsync(projectId: projectId, userId: netUserId, trackedInMinutes: 10, date: date);
        await client.TrackBillableAsync(projectId: projectId, userId: javaUserId, trackedInMinutes: 5, date: date);

        await client.AuthAs(EmailConstants.GroupManagerEmail);

        // Act
        var result = await client.GetPieChartClientsAsync(year: date.Year,
            month: date.Month,
            trackedTimeType: TrackedTimeType.Billable);

        // Assert
        result.Should().NotBeNull();
        result.Should().ContainSingle();
        result[0].Name.Should().Be(projectClient.Name);
        result[0].Color.Should().Be(projectClient.Color);
        result[0].TotalTimeInMinutes.Should().Be(60);

        await client.DeleteTimesheetsUnderCeoRoleAsync(timesheets);
    }
}
