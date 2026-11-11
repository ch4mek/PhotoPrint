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
public class GetTrackedTimePerUsersIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetTrackedTimePerUsersIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetTrackedTimePerUsersWhenEmployeeTriesToAccessOtherUserDataShouldReturnForbiddenResult()
    {
        // Arrange
        _factory.CleanTimesheets();

        var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var result = await client.GetTrackedTimePerUsersReportAsHttpMessageAsync(
            userId: _factory.CtoUserId!.Value,
            year: 2023,
            month: 8);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetTrackedTimePerUsersWhenProjectSpecifiedShouldReturnActiveUsers()
    {
        // Arrange
        _factory.CleanTimesheets();

        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        var firstEmployee = await client.CreateEmployee(departmentId: "Java");
        var firstEmployeeId = firstEmployee!.Id;

        await client.CreateProjectAssignment(userId: firstEmployeeId, projectId: projectId);

        var secondEmployee = await client.CreateEmployee(departmentId: "Java");
        var secondEmployeeId = secondEmployee!.Id;

        await client.CreateProjectAssignment(userId: secondEmployeeId, projectId: projectId);

        //Act
        var result = await client.GetTrackedTimePerUsersReportAsync(
            projectId: projectId,
            trackedTimeType: TrackedTimeType.Fact);

        //Assert
        result.Should().NotBeNull();

        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetTrackedTimePerUsersWhenGroupManagerSpecifiesYearAndMonthShouldReturnTrackedTimePerUserFromResourceGroupForTheSpecifiedPeriod()
    {
        // Arrange
        _factory.CleanTimesheets();

        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var createdUserFromGroup = await client.CreateEmployee(resourceGroupId: 1);
        var userFromGroupId = createdUserFromGroup!.Id;

        var createdUserNotFromGroup = await client.CreateEmployee(resourceGroupId: 2, departmentId: "Java");
        var userNotFromGroupId = createdUserNotFromGroup!.Id;

        var projectWithUserFromSameGroup = await client.CreateProject(clientId: clientId);
        var projectId = projectWithUserFromSameGroup!.Id;

        await client.CreateProjectAssignment(userId: userFromGroupId, projectId: projectId);
        await client.CreateProjectAssignment(userId: userNotFromGroupId, projectId: projectId);

        var timesheets = new List<TimesheetCoreDto>
        {
            await client.TrackFactAsync(userId: userFromGroupId, projectId: projectId, date: date, trackedInMinutes: 480),
            await client.TrackFactAsync(userId: userFromGroupId, projectId: projectId, date: date, trackedInMinutes: 180),

            await client.TrackFactAsync(userId: userNotFromGroupId, projectId: projectId, date: date, trackedInMinutes: 600)
        };

        await client.AuthAsGroupManager();

        // Act
        var result = await client.GetTrackedTimePerUsersReportAsync(
            year: date.Year,
            month: date.Month,
            trackedTimeType: TrackedTimeType.Fact);

        // Assert
        result.Should().NotBeNull();

        result.Items.Should().Contain(x => x.UserId == userFromGroupId);
        result.Items.Should().Contain(x => x.UserId == _factory.GroupManagerId);
        result.Items.Where(user => user.UserId == createdUserNotFromGroup.Id).Should().HaveCount(0);
        var employeeResult = result.Items.First(x => x.UserId == userFromGroupId);
        employeeResult.ProjectsCount.Should().Be(1);
        employeeResult.TotalTimeInMinutes.Should().Be(660);

        await client.DeleteTimesheetsUnderCeoRoleAsync(timesheets);
    }

    [Fact]
    public async Task GetTrackedTimePerUsersWhenTrackedHoursTypeSpecifiedShouldReturnAppropriateProjectCounts()
    {
        // Arrange
        _factory.CleanTimesheets();

        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var user = await client.CreateEmployee();
        var firstProject = await client.CreateProject(clientId: clientId, name: "firstProject GetTrackedTimePerUsersWhenTrackedHoursTypeSpecifiedShouldReturnAppropriateProjectCounts");
        var secondProject = await client.CreateProject(clientId: clientId, isBillable: false, name: "secondProject GetTrackedTimePerUsersWhenTrackedHoursTypeSpecifiedShouldReturnAppropriateProjectCounts");

        await client.CreateProjectAssignment(userId: user!.Id, projectId: firstProject.Id);
        await client.CreateProjectAssignment(userId: user.Id, projectId: secondProject.Id, isTimeBillable: false);

        await client.AuthAs(user.Email);
        await client.TrackFactAsync(userId: user.Id, projectId: firstProject.Id, trackedInMinutes: 480);
        await client.TrackFactAsync(userId: user.Id, projectId: secondProject.Id, trackedInMinutes: 180);

        await client.AuthAsCto();

        // Act
        var factResult = await client.GetTrackedTimePerUsersReportAsync(
            year: date.Year,
            month: date.Month,
            trackedTimeType: TrackedTimeType.Fact);

        var billableResult = await client.GetTrackedTimePerUsersReportAsync(
            year: date.Year,
            month: date.Month,
            trackedTimeType: TrackedTimeType.Billable);

        // Assert
        factResult.Should().NotBeNull();
        billableResult.Should().NotBeNull();

        var factEmployeeResult = factResult.Items.First(x => x.UserId == user.Id);
        factEmployeeResult.ProjectsCount.Should().Be(2);

        var billableEmployeeResult = billableResult.Items.First(x => x.UserId == user.Id);
        billableEmployeeResult.ProjectsCount.Should().Be(1);
    }

    [Fact]
    public async Task GetTrackedTimePerUsersWhenFilterByBillableHoursShouldReturnTrackedTimePerUserForTheSpecifiedPeriod()
    {
        // Arrange
        _factory.CleanTimesheets();

        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var userId = _factory.EmployeeUserId!.Value;
        var headId = _factory.HeadOfDepartmentUserId!.Value;

        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(2));

        var firstProject = await client.CreateProject(clientId: clientId);
        var firstProjectId = firstProject!.Id;

        var secondProject = await client.CreateProject(clientId: clientId, isBillable: false);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: firstProjectId);
        await client.CreateProjectAssignment(userId: userId, projectId: secondProjectId, isTimeBillable: false);

        await client.CreateProjectAssignment(userId: headId, projectId: secondProjectId, isTimeBillable: false);

        await client.UpdateUserPermissionAssignments(userId: userId,
            resources: new Dictionary<string, string>
            {
                { firstProjectId.ToString(CultureInfo.InvariantCulture), firstProject.Name },
                { secondProjectId.ToString(CultureInfo.InvariantCulture), secondProject.Name }
            });

        await client.AuthAsEmployee();
        await client.TrackFactAsync(projectId: firstProjectId, date: date, trackedInMinutes: 500);
        await client.TrackFactAsync(projectId: secondProjectId, date: date, trackedInMinutes: 200);
        await client.TrackFactAsync(projectId: secondProjectId, date: date, trackedInMinutes: 150);

        await client.TrackBillableAsync(projectId: firstProjectId, date: date, trackedInMinutes: 480);
        await client.TrackBillableAsync(projectId: secondProjectId, date: date, trackedInMinutes: 180);
        await client.TrackBillableAsync(projectId: secondProjectId, date: date, trackedInMinutes: 120);

        await client.AuthAsHeadOfDepartment();
        await client.TrackFactAsync(projectId: secondProjectId, date: date, trackedInMinutes: 150);
        await client.TrackFactAsync(projectId: secondProjectId, date: date, trackedInMinutes: 200);

        await client.TrackBillableAsync(projectId: secondProjectId, date: date, trackedInMinutes: 100);
        await client.TrackBillableAsync(projectId: secondProjectId, date: date, trackedInMinutes: 150);

        await client.AuthAsCto();

        // Act
        var result = await client.GetTrackedTimePerUsersReportAsync(
            year: date.Year,
            month: date.Month,
            trackedTimeType: TrackedTimeType.Billable);

        // Assert
        result.Should().NotBeNull();

        var employeeResult = result.Items.First(x => x.UserId == _factory.EmployeeUserId);
        employeeResult.ProjectsCount.Should().Be(2);
        employeeResult.TotalTimeInMinutes.Should().Be(780);

        var headResult = result.Items.First(x => x.UserId == _factory.HeadOfDepartmentUserId);
        headResult.ProjectsCount.Should().Be(1);
        headResult.TotalTimeInMinutes.Should().Be(250);
    }
}
