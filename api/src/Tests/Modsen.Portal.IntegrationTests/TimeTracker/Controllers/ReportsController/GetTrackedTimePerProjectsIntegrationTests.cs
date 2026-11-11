using System.Net;
using System.Text.Json;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.IntegrationTests.TestFramework.IntegrationTestFactoryExtensions;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.ReportsController;

[Collection("TimeTrackerCollection")]
public class GetTrackedTimePerProjectsIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetTrackedTimePerProjectsIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _factory.CleanTimesheets();
    }

    [Fact]
    public async Task GetTrackedTimePerProjectsWhenEmployeeTriesToAccessOtherUserDataShouldReturnForbiddenResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var response = await client.GetTrackedTimePerProjectReportAsHttpMessageAsync(_factory.CtoUserId!.Value);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetTrackedTimePerProjectsWhenNoParametersSpecifiedShouldReturnTrackedTimeForAllProjects()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var employee = await client.CreateEmployee();
        var userId = employee!.Id;

        var firstProject = await client.CreateProject(clientId: clientId);
        var firstProjectId = firstProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: firstProjectId);

        var secondProject = await client.CreateProject(clientId: clientId, isBillable: false);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: secondProjectId);

        await client.TrackFactAsync(projectId: secondProjectId, userId: userId, trackedInMinutes: 180,
            date: date.AddDays(8));
        await client.TrackFactAsync(projectId: secondProjectId, userId: userId, trackedInMinutes: 120,
            date: date.AddMonths(1));

        await client.AuthAs(employee.Email);
        await client.TrackFactAsync(projectId: firstProjectId, trackedInMinutes: 480, date: date);

        // Act
        var result = await client.GetTrackedTimePerProjectReportAsync(userId: userId,
            trackedTimeType: TrackedTimeType.Fact);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        result.Should().Contain(x => x.ProjectId == firstProjectId);
        var project1Result = result.First(x => x.ProjectId == firstProjectId);
        project1Result.TasksCount.Should().Be(1);
        project1Result.TotalTimeInMinutes.Should().Be(480);

        result.Should().Contain(x => x.ProjectId == secondProjectId);
        var project2Result = result.First(x => x.ProjectId == secondProjectId);
        project2Result.TasksCount.Should().Be(2);
        project2Result.TotalTimeInMinutes.Should().Be(300);
    }

    [Fact]
    public async Task GetTrackedTasksByUserAndProjectWhenUserHasTrackedTaskWithoutBillableTimeShouldReturnEmptyTrackedTimePerProjectReportOutputList()
    {
        // Arrange
        var client = _factory.CreateClient();
        var userId = _factory.AdminUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;

        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        await client.AuthAsCeo();
        var project = await client.CreateProject(clientId: clientId, isBillable: false);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId, isTimeBillable: false);

        var trackedTask = new UpsertFactDto
        {
            ProjectId = project.Id,
            ReportForDate = date,
            TaskName = "First task",
            FactTrackedInMinutes = 480
        };

        await client.AuthAsAdmin();
        await client.CreateFactTimesheetAsHttpMessageAsync(trackedTask);

        await client.AuthAsCeo();

        // Act
        var result = await client.GetTrackedTimePerProjectReportAsync(userId: userId,
            trackedTimeType: TrackedTimeType.Billable);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTrackedTimePerProjectsWhenYearSpecifiedShouldReturnTrackedTimeForAllProjectsWithinSpecifiedPeriod()
    {
        // Arrange
        var client = _factory.CreateClient();
        var userId = _factory.HeadOfDepartmentUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;

        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        await client.AuthAsHeadOfDepartment();
        var firstProject = await client.CreateProject(clientId: clientId);
        var firstProjectId = firstProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: firstProjectId);

        var secondProject = await client.CreateProject(clientId: clientId, isBillable: false);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: secondProjectId);

        await client.TrackFactAsync(projectId: firstProjectId, userId: userId, date: date.AddYears(1),
            trackedInMinutes: 480);

        await client.TrackFactAsync(projectId: secondProjectId, date: date, trackedInMinutes: 180);
        await client.TrackFactAsync(projectId: secondProjectId, date: date, trackedInMinutes: 120);

        // Act
        var result = await client.GetTrackedTimePerProjectReportAsync(userId: userId,
            year: date.Year,
            trackedTimeType: TrackedTimeType.Fact);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThanOrEqualTo(1);

        result.Should().Contain(x => x.ProjectId == secondProjectId);
        var project2Result = result.First(x => x.ProjectId == secondProjectId);
        project2Result.TasksCount.Should().Be(2);
        project2Result.TotalTimeInMinutes.Should().Be(300);
    }

    [Fact]
    public async Task GetTrackedTimePerProjectsWhenYearAndMonthSpecifiedShouldReturnTrackedTimeForAllProjectsWithinSpecifiedPeriod()
    {
        // Arrange
        var client = _factory.CreateClient();
        var userId = _factory.HeadOfDepartmentUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;

        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        await client.AuthAsCeo();
        var firstProject = await client.CreateProject(clientId: clientId);
        var firstProjectId = firstProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: firstProjectId);

        var secondProject = await client.CreateProject(clientId: clientId, isBillable: false);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: secondProjectId);

        await client.TrackFactAsync(projectId: firstProjectId, userId: userId, date: date.AddMonths(1),
            trackedInMinutes: 480);

        await client.TrackFactAsync(projectId: secondProjectId, userId: userId, date: date.AddMonths(2),
            trackedInMinutes: 180);

        await client.AuthAsHeadOfDepartment();
        await client.TrackFactAsync(projectId: secondProjectId, date: date, trackedInMinutes: 120);

        // Act
        var result = await client.GetTrackedTimePerProjectReportAsync(userId: userId,
            year: date.Year,
            month: date.Month,
            trackedTimeType: TrackedTimeType.Fact);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThanOrEqualTo(1);

        result.Should().Contain(x => x.ProjectId == secondProjectId);
        var project2Result = result.First(x => x.ProjectId == secondProjectId);
        project2Result.TasksCount.Should().Be(1);
        project2Result.TotalTimeInMinutes.Should().Be(120);
    }

    [Fact]
    public async Task GetTrackedTimePerProjectsWhenYearAndMonthSpecifiedByGroupManagerShouldReturnTrackedTimeForAllProjectsWithinSpecifiedPeriod()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var clientId = _factory.ModsenClientId!.Value;

        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var createdUserFromGroup = await client.CreateEmployee(resourceGroupId: 1);
        var userId = createdUserFromGroup!.Id;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        await client.TrackFactAsync(projectId: projectId, userId: userId, trackedInMinutes: 480,
            date: date.AddMonths(1));

        await client.TrackFactAsync(projectId: projectId, userId: userId, trackedInMinutes: 555, date: date);

        await client.AuthAs(EmailConstants.GroupManagerEmail);

        // Act
        var result = await client.GetTrackedTimePerProjectReportAsync(userId: userId,
            year: date.Year,
            month: date.Month,
            trackedTimeType: TrackedTimeType.Fact);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThanOrEqualTo(1);

        result.Should().Contain(x => x.ProjectId == projectId);
        var projectResult = result.First(x => x.ProjectId == projectId);
        projectResult.TasksCount.Should().Be(1);
        projectResult.TotalTimeInMinutes.Should().Be(555);
    }

    [Fact]
    public async Task GetTrackedTimePerProjectsWhenYearAndWeekSpecifiedShouldReturnTrackedTimeForAllProjectsWithinSpecifiedPeriod()
    {
        // Arrange
        var client = _factory.CreateClient();
        var userId = _factory.HeadOfDepartmentUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;

        var date = DateTime.UtcNow.Date;
        var week = date.GetWeekOfYear();

        await client.AuthAsCeo();
        var firstProject = await client.CreateProject(clientId: clientId);
        var firstProjectId = firstProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: firstProjectId);

        var secondProject = await client.CreateProject(clientId: clientId, isBillable: false);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: secondProjectId);

        await client.TrackFactAsync(projectId: secondProjectId, userId: userId,
            date: DateOnly.FromDateTime(date.AddDays(8)), trackedInMinutes: 120);

        await client.AuthAsHeadOfDepartment();
        await client.TrackFactAsync(projectId: firstProjectId, trackedInMinutes: 480);
        await client.TrackFactAsync(projectId: secondProjectId, trackedInMinutes: 180);

        // Act
        var result = await client.GetTrackedTimePerProjectReportAsync(userId: userId,
            year: date.Year,
            week: week,
            trackedTimeType: TrackedTimeType.Fact);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().BeGreaterThanOrEqualTo(2);

        result.Should().Contain(x => x.ProjectId == firstProjectId);
        var project1Result = result.First(x => x.ProjectId == firstProjectId);
        project1Result.TasksCount.Should().Be(1);
        project1Result.TotalTimeInMinutes.Should().Be(480);

        result.Should().Contain(x => x.ProjectId == secondProjectId);
        var project2Result = result.First(x => x.ProjectId == secondProjectId);
        project2Result.TasksCount.Should().Be(1);
        project2Result.TotalTimeInMinutes.Should().Be(180);
    }

    [Fact]
    public async Task GetTrackedTimePerProjectsWhenFilterByBillableHoursShouldReturnTrackedTimeForAllProjectsWithinSpecifiedPeriod()
    {
        // Arrange
        var client = _factory.CreateClient();
        var userId = _factory.HeadOfDepartmentUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;

        var date = DateTime.UtcNow.Date;

        await client.AuthAsCeo();
        var firstProject = await client.CreateProject(clientId: clientId);
        var firstProjectId = firstProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: firstProjectId);

        var secondProject = await client.CreateProject(clientId: clientId, isBillable: false);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: secondProjectId);

        await client.AuthAsHeadOfDepartment();
        await client.TrackFactAsync(projectId: firstProjectId, trackedInMinutes: 500);
        await client.TrackFactAsync(projectId: secondProjectId, trackedInMinutes: 200);

        var timesheets = new List<TimesheetCoreDto>
        {
            await client.TrackBillableAsync(projectId: firstProjectId, trackedInMinutes: 480),
            await client.TrackBillableAsync(projectId: secondProjectId, trackedInMinutes: 180)
        };

        // Act
        var result = await client.GetTrackedTimePerProjectReportAsync(
            userId: userId,
            year: date.Year,
            trackedTimeType: TrackedTimeType.Billable);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        result.Should().Contain(x => x.ProjectId == firstProjectId);
        var project1Result = result.First(x => x.ProjectId == firstProjectId);
        project1Result.TasksCount.Should().Be(1);
        project1Result.TotalTimeInMinutes.Should().Be(480);

        result.Should().Contain(x => x.ProjectId == secondProjectId);
        var project2Result = result.First(x => x.ProjectId == secondProjectId);
        project2Result.TasksCount.Should().Be(1);
        project2Result.TotalTimeInMinutes.Should().Be(180);

        await client.DeleteTimesheetsUnderCeoRoleAsync(timesheets);
    }
}
