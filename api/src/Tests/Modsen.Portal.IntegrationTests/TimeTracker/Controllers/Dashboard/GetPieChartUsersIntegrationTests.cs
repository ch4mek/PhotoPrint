using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.IntegrationTests.TestFramework.IntegrationTestFactoryExtensions;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.Dashboard;

[Collection("TimeTrackerCollection")]
public class GetPieChartUsersIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetPieChartUsersIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _factory.CleanTimesheets();
    }

    [Fact]
    public async Task GetPieChartUsersWhenEmployeeTriesToAccessOtherUserDataShouldReturnForbiddenResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var ctoUserId = _factory.CtoUserId!.Value;

        // Act
        var result = await client.GetDashboardPieChartAsHttpMessageAsync(userId: ctoUserId);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetPieChartUsersWhenYearAndMonthSpecifiedShouldReturnUsersWithTotalTrackedTimeForTheSpecifiedPeriod()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var clientId = _factory.ModsenClientId!.Value;
        var headId = _factory.HeadOfDepartmentUserId!.Value;

        var createdUser = await client.CreateEmployee();
        var userId = createdUser!.Id;

        var firstProject = await client.CreateProject(clientId: clientId);
        var firstProjectId = firstProject!.Id;

        var secondProject = await client.CreateProject(clientId: clientId, isBillable: false);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: firstProjectId);
        await client.CreateProjectAssignment(userId: userId, projectId: secondProjectId);
        await client.CreateProjectAssignment(userId: headId, projectId: secondProjectId);

        await client.AuthAs(createdUser.Email);

        var userTimeSheets = new List<TimesheetCoreDto>
        {
            await client.TrackFactAsync(projectId: firstProjectId, trackedInMinutes: 480),
            await client.TrackFactAsync(projectId: secondProjectId, trackedInMinutes: 180),
            await client.TrackFactAsync(projectId: secondProjectId, trackedInMinutes: 120),
        };

        await client.AuthAsHeadOfDepartment();

        var headTimeSheets = new List<TimesheetCoreDto>
        {
            await client.TrackFactAsync(projectId: secondProjectId, trackedInMinutes: 100),
            await client.TrackFactAsync(projectId: secondProjectId, trackedInMinutes: 150)
        };

        await client.AuthAsCto();

        // Act
        var result = await client.GetDashboardPieChartAsync();

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);

        var employee = result.Find(u => u.FirstName == createdUser.FirstName && u.LastName == createdUser.LastName);
        employee.Should().NotBeNull();
        employee.TrackedTime.Should().Be(780);

        var head = result.Find(u => u is { FirstName: "Test", LastName: "HeadOfDepartment" });
        head.Should().NotBeNull();
        head.TrackedTime.Should().Be(250);

        await client.DeleteTimesheetsUnderCeoRoleAsync([..userTimeSheets.Union(headTimeSheets)]);
    }
}
