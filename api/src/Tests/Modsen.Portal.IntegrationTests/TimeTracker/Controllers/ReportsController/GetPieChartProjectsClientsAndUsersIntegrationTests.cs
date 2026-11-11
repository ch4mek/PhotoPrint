using System.Net;
using System.Text.Json;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.ReportsController;

[Collection("TimeTrackerCollection")]
public class GetPieChartProjectsClientsAndUsersIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetPieChartProjectsClientsAndUsersIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetPieChartProjectsClientsAndUsersWhenAuthAsEmployeeShouldReturnForbiddenResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var action = await client.GetPieChartReportsAsHttpMessageAsync();

        // Assert
        action.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetPieChartProjectsClientsAndUsersWhenAuthAsHeadShouldReturnHeadDepartmentProjectsTimeForTheSpecifiedPeriod()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var createdNetEmployee = await client.CreateEmployee();
        var userId = createdNetEmployee!.Id;

        var projectClient = await client.CreateClientAsync();
        var clientId = projectClient!.Id;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        await client.AuthAs(createdNetEmployee.Email);
        await client.TrackFactAsync(projectId: projectId, trackedInMinutes: 150, date: date);
        await client.TrackFactAsync(projectId: projectId, trackedInMinutes: 100, date: date);

        // Act
        await client.AuthAsHeadOfDepartment();
        var result = await client.GetPieChartReportsAsync(year: date.Year,
            month: date.Month,
            trackedTimeType: TrackedTimeType.Fact);

        // Assert
        result.Should().NotBeNull();
        result.PerProjects.Should().NotBeEmpty();
        result.PerClients.Should().NotBeEmpty();
        result.PerUsers.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetPieChartProjectsClientsAndUsersWhenUserDoesNotHaveBillableTrackedTimeShouldReturnEmptyCollections()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var clientId = _factory.ModsenClientId!.Value;

        var createdUser = await client.CreateEmployee();
        var userId = createdUser!.Id;

        var firstProject = await client.CreateProject(clientId: clientId, isBillable: false);
        var firstProjectId = firstProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: firstProjectId, isTimeBillable: false);

        await client.AuthAs(createdUser.Email);

        var timesheets = new List<TimesheetCoreDto>
        {
            await client.TrackFactAsync(projectId: firstProjectId, trackedInMinutes: 480, date: date)
        };

        // Act
        await client.AuthAsCto();

        var result = await client.GetPieChartReportsAsync(
            userId: userId,
            year: date.Year,
            month: date.Month,
            trackedTimeType: TrackedTimeType.Billable);

        // Assert
        result.Should().NotBeNull();
        result.PerUsers.Should().BeEmpty();
        result.PerProjects.Should().BeEmpty();
        result.PerClients.Should().BeEmpty();

        await client.DeleteTimesheetsUnderCeoRoleAsync(timesheets);
    }
}
