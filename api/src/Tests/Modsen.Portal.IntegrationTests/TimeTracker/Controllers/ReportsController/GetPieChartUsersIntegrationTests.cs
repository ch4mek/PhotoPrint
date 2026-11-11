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
        var userId = _factory.CtoUserId!.Value;

        // Act
        var result = await client.GetPieChartUsersAsHttpMessageAsync(userId: userId);

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
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var head = await client.GetUserAsync(headId);

        var createdUser = await client.CreateEmployee();
        var userId = createdUser!.Id;

        var firstProject = await client.CreateProject(clientId: clientId);
        var firstProjectId = firstProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: firstProjectId);

        var secondProject = await client.CreateProject(clientId: clientId, isBillable: false);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: secondProjectId);
        await client.CreateProjectAssignment(userId: headId, projectId: secondProjectId);

        await client.AuthAs(createdUser.Email);

        var timesheets = new List<TimesheetCoreDto>
        {
            await client.TrackFactAsync(projectId: firstProjectId, trackedInMinutes: 480),
            await client.TrackFactAsync(projectId: secondProjectId, trackedInMinutes: 180),
            await client.TrackFactAsync(projectId: secondProjectId, trackedInMinutes: 120)
        };

        await client.AuthAsHeadOfDepartment();

        timesheets.Add(await client.TrackFactAsync(projectId: secondProjectId, trackedInMinutes: 100));
        timesheets.Add(await client.TrackFactAsync(projectId: secondProjectId, trackedInMinutes: 150));

        await client.AuthAsCto();

        // Act
        var result = await client.GetPieChartUsersAsync(year: date.Year,
            month: date.Month,
            trackedTimeType: TrackedTimeType.Fact);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);

        var employee = result.Find(u =>
            u.FirstName == createdUser.FirstName && u.LastName == createdUser.LastName);
        employee.Should().NotBeNull();
        employee.TotalTimeInMinutes.Should().Be(780);

        var headResult = result.Find(u =>
            u.FirstName == head!.FirstName && u.LastName == head.LastName);
        headResult.Should().NotBeNull();
        headResult.TotalTimeInMinutes.Should().Be(250);

        await client.DeleteTimesheetsUnderCeoRoleAsync(timesheets);
    }

    [Fact]
    public async Task GetPieChartUsersWhenTakeBillableHoursShouldReturnUsersWithTotalTrackedTimeForTheSpecifiedPeriod()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var headId = _factory.HeadOfDepartmentUserId!.Value;
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1));

        var head = await client.GetUserAsync(headId);

        var createdUser = await client.CreateEmployee();
        var userId = createdUser!.Id;

        var firstProject = await client.CreateProject(clientId: clientId);
        var firstProjectId = firstProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: firstProjectId);

        var secondProject = await client.CreateProject(clientId: clientId, isBillable: false);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: secondProjectId, isTimeBillable: false);
        await client.CreateProjectAssignment(userId: headId, projectId: secondProjectId, isTimeBillable: false);

        await client.UpdateUserPermissionAssignments(userId: userId,
            resources: new Dictionary<string, string>
            {
                { firstProjectId.ToString(CultureInfo.InvariantCulture), firstProject.Name }
            });

        await client.AuthAs(createdUser.Email);

        await client.TrackFactAsync(projectId: firstProjectId, trackedInMinutes: 480, date: date);

        const int employeeBillableTrackedTime = 480;
        var timesheets = new List<TimesheetCoreDto>
        {
            await client.TrackFactAsync(projectId: secondProjectId, trackedInMinutes: 180, date: date),
            await client.TrackFactAsync(projectId: secondProjectId, trackedInMinutes: 120, date: date),
            await client.TrackBillableAsync(projectId: firstProjectId, trackedInMinutes: employeeBillableTrackedTime,
                date: date)
        };

        await client.AuthAsHeadOfDepartment();

        const int headBillableTrackedTime = 480;
        timesheets.Add(await client.TrackFactAsync(projectId: secondProjectId, trackedInMinutes: 100, date: date));
        timesheets.Add(await client.TrackFactAsync(projectId: secondProjectId, trackedInMinutes: 150, date: date));
        timesheets.Add(await client.TrackBillableAsync(projectId: secondProjectId,
            trackedInMinutes: headBillableTrackedTime, date: date));

        await client.AuthAsCto();

        // Act
        var result = await client.GetPieChartUsersAsync(year: date.Year,
            month: date.Month,
            trackedTimeType: TrackedTimeType.Billable);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);

        var employee = result.Find(u =>
            u.FirstName == createdUser.FirstName && u.LastName == createdUser.LastName);
        employee.Should().NotBeNull();
        employee.TotalTimeInMinutes.Should().Be(employeeBillableTrackedTime);

        var headResult = result.Find(u =>
            u.FirstName == head!.FirstName && u.LastName == head.LastName);
        headResult.Should().NotBeNull();
        headResult.TotalTimeInMinutes.Should().Be(headBillableTrackedTime);

        await client.DeleteTimesheetsUnderCeoRoleAsync(timesheets);
    }
}
