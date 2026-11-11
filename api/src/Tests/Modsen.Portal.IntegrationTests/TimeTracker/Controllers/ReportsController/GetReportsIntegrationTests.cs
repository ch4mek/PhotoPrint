using System.Globalization;
using System.Text.Json;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.ReportsController;

[Collection("TimeTrackerCollection")]
public class GetReportsIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetReportsIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetTrackedTimePerPeriodWhenUserHasThreeTrackedTasksShouldReturnTrackedTimeForTheSpecifiedPeriod()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var userId = _factory.EmployeeUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;

        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        const int firstTrackedTime = 480;
        const int secondTrackedTime = 360;

        await client.TrackFactAsync(projectId: projectId, userId: userId, trackedInMinutes: 120,
            date: date.AddMonths(1));
        await client.TrackFactAsync(projectId: projectId, userId: userId, trackedInMinutes: 180,
            date: date.AddYears(1));

        const int expectedTrackedTime = firstTrackedTime + secondTrackedTime;

        await client.AuthAsEmployee();
        await client.TrackFactAsync(projectId: projectId, trackedInMinutes: firstTrackedTime, date: date);
        await client.TrackFactAsync(projectId: projectId, trackedInMinutes: secondTrackedTime, date: date);

        await client.AuthAsCeo();

        // Act
        var result = await client.GetTrackedTimePerPeriodReportAsync(
            userId: userId,
            projectId: projectId,
            trackedTimeType: TrackedTimeType.Fact,
            year: date.Year,
            month: date.Month);

        // Assert
        result.Should().NotBeNull();
        result.TotalTimeInMinutes.Should().Be(expectedTrackedTime);
        result.TimePerPeriod.First(x => DateOnly.FromDateTime(x.Period.Date) == date)
            .TotalTimeInMinutes.Should().Be(expectedTrackedTime);
    }

    [Fact]
    public async Task GetTrackedTasksByUserAndProjectWhenNoParametersSpecifiedShouldReturnAllTrackedTasksForProject()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var userId = _factory.EmployeeUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;

        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        var secondTrackedTask = await client.TrackFactAsync(projectId: projectId, userId: userId, trackedInMinutes: 360,
            date: date.AddMonths(1));

        var thirdTrackedTask = await client.TrackFactAsync(projectId: projectId, userId: userId, trackedInMinutes: 120,
            date: date.AddYears(1));

        await client.AuthAsEmployee();
        var firstTrackedTask = await client.TrackFactAsync(projectId: projectId, trackedInMinutes: 480);

        await client.AuthAsCeo();

        // Act
        var result = await client.GetTrackedTasksReportAsync(
            userId: userId,
            projectId: projectId,
            trackedTimeType: TrackedTimeType.Fact);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(x => x.TaskName == firstTrackedTask.TaskName);
        result.Should().Contain(x => x.TaskName == secondTrackedTask.TaskName);
        result.Should().Contain(x => x.TaskName == thirdTrackedTask.TaskName);
    }

    [Fact]
    public async Task GetTrackedTasksByUserAndProjectWhenYearSpecifiedShouldReturnTrackedTasksForSpecifiedYear()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var userId = _factory.EmployeeUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;

        var date = new DateOnly(2024, 11, 11);

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, onProjectFrom: date, projectId: projectId);

        var firstTrackedTask = await client.TrackFactAsync(projectId: projectId, userId: userId,
            trackedInMinutes: 480, date: date);
        var secondTrackedTask = await client.TrackFactAsync(projectId: projectId, userId: userId,
            trackedInMinutes: 360, date: date.AddDays(1));
        await client.TrackFactAsync(projectId: projectId, userId: userId, trackedInMinutes: 120, date:
            date.AddYears(1));

        await client.TrackBillableAsync(projectId: projectId, userId: userId, trackedInMinutes: 480, date: date);
        await client.TrackBillableAsync(projectId: projectId, userId: userId, trackedInMinutes: 360, date: date.AddDays(1));

        await client.AuthAsCeo();

        // Act
        var result = await client.GetTrackedTasksReportAsync(
            userId: userId,
            projectId: projectId,
            year: date.Year,
            trackedTimeType: TrackedTimeType.Fact);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(x => x.TaskName == firstTrackedTask.TaskName);
        result.Should().Contain(x => x.TaskName == secondTrackedTask.TaskName);
    }

    [Fact]
    public async Task GetTrackedTasksByUserAndProjectWhenUserHasTrackedTaskWithoutBillableTimeShouldReturnEmptyTrackedTaskReportOutputList()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var userId = _factory.EmployeeUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;

        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var project = await client.CreateProject(clientId: clientId, isBillable: false);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        await client.AuthAsEmployee();
        await client.CreateFactTimesheetAsHttpMessageAsync(projectId: projectId,
            date: date,
            factTrackedInMinutes: 480,
            billableTrackedInMinutes: 0);

        // Act
        await client.AuthAsCeo();

        var result = await client.GetTrackedTasksReportAsync(
            userId: userId,
            projectId: projectId,
            year: date.Year,
            trackedTimeType: TrackedTimeType.Billable);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTrackedTasksByUserAndProjectWhenYearAndMonthSpecifiedShouldReturnTrackedTasksForSpecifiedMonth()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var userId = _factory.EmployeeUserId!.Value;

        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        await client.TrackFactToTimesheetManagerAsHttpMessageAsync(projectId: projectId, trackedInMinutes: 360, date: date.AddMonths(1),
            userId: userId);
        await client.TrackFactToTimesheetManagerAsHttpMessageAsync(projectId: projectId, trackedInMinutes: 120, date: date.AddYears(1),
            userId: userId);

        await client.AuthAsEmployee();
        var firstTrackedTask = await client.TrackFactAsync(projectId: projectId,
            trackedInMinutes: 480, date: date);

        await client.AuthAsCeo();

        // Act
        var result = await client.GetTrackedTasksReportAsync(userId: userId,
            projectId: projectId,
            year: date.Year,
            month: date.Month,
            trackedTimeType: TrackedTimeType.Fact);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.Should().Contain(x => x.TaskName == firstTrackedTask.TaskName);
    }

    [Fact]
    public async Task GetTrackedTasksByUserAndProjectWhenYearAndWeekSpecifiedShouldReturnTrackedTasksForSpecifiedWeek()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var userId = _factory.EmployeeUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;

        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var week = date.GetWeekOfYear();

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        await client.TrackFactToTimesheetManagerAsHttpMessageAsync(projectId: projectId, userId: userId, trackedInMinutes: 180,
            date: date.AddDays(8));

        await client.AuthAsEmployee();
        var firstTrackedTask = await client.TrackFactAsync(projectId: projectId,
            trackedInMinutes: 480, date: date);
        var thirdTrackedTask = await client.TrackFactAsync(projectId: projectId,
            trackedInMinutes: 120, date: date);

        await client.AuthAsCeo();

        // Act
        var result = await client.GetTrackedTasksReportAsync(userId: userId,
            projectId: projectId,
            year: date.Year,
            week: week,
            trackedTimeType: TrackedTimeType.Fact);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(x => x.TaskName == firstTrackedTask.TaskName);
        result.Should().Contain(x => x.TaskName == thirdTrackedTask.TaskName);
    }

    [Fact]
    public async Task GetTrackedTasksByUserAndProjectWhenTakeBillableHoursShouldReturnTrackedTasksForSpecifiedWeek()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var userId = _factory.EmployeeUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;

        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var week = date.GetWeekOfYear();

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        await client.UpdateUserPermissionAssignments(userId: userId,
            resources: new Dictionary<string, string>
            {
                { projectId.ToString(CultureInfo.InvariantCulture), project.Name}
            });

        await client.CreateTimesheetToTimesheetManagerAsHttpMessageAsync(projectId: projectId,
            date: date.AddDays(8),
            factTrackedInMinutes: 200,
            billableTrackedInMinutes: 180,
            userId: userId);

        await client.AuthAsEmployee();
        var timesheets = new List<TimesheetCoreDto>
        {
            await client.TrackFactAsync(projectId: projectId, trackedInMinutes: 500, date: date),
            await client.TrackFactAsync(projectId: projectId, trackedInMinutes: 150, date: date)
        };

        var firstTrackedTaskBillable = await client.TrackBillableAsync(projectId: projectId,
            trackedInMinutes: 480, date: date);
        var thirdTrackedTaskBillable = await client.TrackBillableAsync(projectId: projectId,
            trackedInMinutes: 120, date: date);

        timesheets.AddRange([firstTrackedTaskBillable, thirdTrackedTaskBillable]);

        await client.AuthAsCeo();

        // Act
        var result = await client.GetTrackedTasksReportAsync(userId: userId,
            projectId: projectId,
            year: date.Year,
            week: week,
            trackedTimeType: TrackedTimeType.Billable);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(x => x.TrackedTimeInMinutes == firstTrackedTaskBillable.BillableTrackedInMinutes);
        result.Should().Contain(x => x.TrackedTimeInMinutes == thirdTrackedTaskBillable.BillableTrackedInMinutes);

        await client.DeleteTimesheetsUnderCeoRoleAsync(timesheets);
    }
}
