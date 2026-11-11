using System.Globalization;
using System.Net;
using System.Text.Json;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.IntegrationTests.TestFramework.IntegrationTestFactoryExtensions;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output.Reports;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.ReportsController;

[Collection("TimeTrackerCollection")]
public class GetPieChartProjectsIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetPieChartProjectsIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _factory.CleanTimesheets();
    }

    [Fact]
    public async Task GetPieChartProjectsWhenAuthAsEmployeeShouldReturnForbiddenResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var action = await client.GetPieChartProjectsAsHttpMessageAsync();

        // Assert
        action.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetPieChartProjectsWhenAuthAsHeadShouldReturnHeadDepartmentProjectsTimeForTheSpecifiedPeriod()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var headId = _factory.HeadOfDepartmentUserId!.Value;

        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var createdUser = await client.CreateEmployee();
        var userId = createdUser!.Id;

        var firstProject = await client.CreateProject(clientId: clientId);
        var firstProjectId = firstProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: firstProjectId);

        var secondProject = await client.CreateProject(clientId: clientId);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: secondProjectId);
        await client.CreateProjectAssignment(userId: headId, projectId: secondProjectId);

        var expectedResult = new List<ProjectWithTrackedTimeReportOutput>
        {
            new()
            {
                Name = firstProject.Name,
                Color = firstProject.Color,
                TotalTimeInMinutes = 150
            },
            new()
            {
                Name = secondProject.Name,
                Color = secondProject.Color,
                TotalTimeInMinutes = 220
            }
        };

        await client.AuthAs(createdUser.Email);

        var timesheets = new List<TimesheetCoreDto>
        {
            await client.TrackFactAsync(projectId: firstProjectId, trackedInMinutes: 150, date: date),
            await client.TrackFactAsync(projectId: secondProjectId, trackedInMinutes: 120, date: date),
            await client.TrackFactAsync(projectId: secondProjectId, trackedInMinutes: 100, date: date)
        };

        await client.AuthAsHeadOfDepartment();

        // Act
        var result = await client.GetPieChartProjectsAsync(year: date.Year,
            month: date.Month,
            trackedTimeType: TrackedTimeType.Billable);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResult, opt => opt.WithoutStrictOrdering());

        await client.DeleteTimesheetsUnderCeoRoleAsync(timesheets);
    }

    [Fact]
    public async Task GetPieChartProjectsWhenAuthAsCeoShouldReturnProjectsTimeForTheSpecifiedPeriod()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var ceoId = _factory.CeoUserId!.Value;

        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1).Date);

        var createdUser = await client.CreateEmployee();
        var userId = createdUser!.Id;

        var firstProject = await client.CreateProject(clientId: clientId);
        var firstProjectId = firstProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: firstProjectId);

        var secondProject = await client.CreateProject(clientId: clientId, isBillable: false);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: secondProjectId);
        await client.CreateProjectAssignment(userId: ceoId, projectId: secondProjectId);

        var expectedResult = new List<ProjectWithTrackedTimeReportOutput>
        {
            new()
            {
                Name = firstProject.Name,
                Color = firstProject.Color,
                TotalTimeInMinutes = 480
            },
            new()
            {
                Name = secondProject.Name,
                Color = secondProject.Color,
                TotalTimeInMinutes = 270
            }
        };

        await client.AuthAs(createdUser.Email);

        var timesheets = new List<TimesheetCoreDto>
        {
            await client.TrackFactAsync(projectId: firstProjectId, trackedInMinutes: 480, date: date),
            await client.TrackFactAsync(projectId: secondProjectId, trackedInMinutes: 120, date: date),
            await client.TrackFactAsync(projectId: secondProjectId, trackedInMinutes: 150, date: date)
        };

        await client.AuthAsCeo();

        // Act
        var result = await client.GetPieChartProjectsAsync(year: date.Year,
            month: date.Month,
            trackedTimeType: TrackedTimeType.Fact);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResult, opt => opt.WithoutStrictOrdering());

        await client.DeleteTimesheetsUnderCeoRoleAsync(timesheets);
    }

    [Fact]
    public async Task GetPieChartProjectsWhenTakeBillableHoursShouldReturnProjectsTimeForTheSpecifiedPeriod()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var ceoId = _factory.CeoUserId!.Value;

        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(2).Date);

        var createdUser = await client.CreateEmployee();
        var userId = createdUser!.Id;

        var firstProject = await client.CreateProject(clientId: clientId);
        var firstProjectId = firstProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: firstProjectId);

        var secondProject = await client.CreateProject(clientId: clientId, isBillable: false);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: secondProjectId);
        await client.CreateProjectAssignment(userId: ceoId, projectId: secondProjectId);

        await client.UpdateUserPermissionAssignments(userId: userId,
            resources: new Dictionary<string, string>
            {
                { firstProjectId.ToString(CultureInfo.InvariantCulture), firstProject.Name },
                { secondProjectId.ToString(CultureInfo.InvariantCulture), secondProject.Name }
            });

        const int firstTrackedBillableTime = 480;
        const int secondTrackedBillableTime= 120;
        const int thirdTrackedBillableTime= 150;

        var expectedResult = new List<ProjectWithTrackedTimeReportOutput>
        {
            new()
            {
                Name = firstProject.Name,
                Color = firstProject.Color,
                TotalTimeInMinutes = firstTrackedBillableTime
            },
            new()
            {
                Name = secondProject.Name,
                Color = secondProject.Color,
                TotalTimeInMinutes = secondTrackedBillableTime + thirdTrackedBillableTime
            }
        };

        await client.AuthAs(createdUser.Email);

        await client.TrackFactAsync(projectId: firstProjectId, trackedInMinutes: 580, date: date);
        await client.TrackFactAsync(projectId: secondProjectId, trackedInMinutes: 150, date: date);
        await client.TrackFactAsync(projectId: secondProjectId, trackedInMinutes: 180, date: date);

        var timesheets = new List<TimesheetCoreDto>
        {
            await client.TrackBillableAsync(projectId: firstProjectId, trackedInMinutes: firstTrackedBillableTime,
                date: date),
            await client.TrackBillableAsync(projectId: secondProjectId, trackedInMinutes: secondTrackedBillableTime,
                date: date),
            await client.TrackBillableAsync(projectId: secondProjectId, trackedInMinutes: thirdTrackedBillableTime,
                date: date),
        };

        await client.AuthAsCeo();

        // Act
        var result = await client.GetPieChartProjectsAsync(year: date.Year,
            month: date.Month,
            trackedTimeType: TrackedTimeType.Billable);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResult, opt => opt.WithoutStrictOrdering());

        await client.DeleteTimesheetsUnderCeoRoleAsync(timesheets);
    }

    [Fact]
    public async Task GetPieChartProjectsWhenAuthAsGmShouldReturnGroupManagersProjectsTimeForTheSpecifiedPeriod()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var groupManagerId = _factory.GroupManagerId!.Value;

        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var createdUserFromGroup = await client.CreateEmployee(resourceGroupId: 1);
        var netUserId = createdUserFromGroup!.Id;
        var createdUserNotFromGroup = await client.CreateEmployee(departmentId: "Java", resourceGroupId: 2);
        var javaUserId = createdUserNotFromGroup!.Id;

        var firstProject = await client.CreateProject(clientId: clientId);
        var firstProjectId = firstProject!.Id;

        await client.CreateProjectAssignment(userId: netUserId, projectId: firstProjectId);
        await client.CreateProjectAssignment(userId: javaUserId, projectId: firstProjectId);

        var secondProject = await client.CreateProject(clientId: clientId);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: netUserId, projectId: secondProjectId);
        await client.CreateProjectAssignment(userId: groupManagerId, projectId: secondProjectId);

        var expectedResult = new List<ProjectWithTrackedTimeReportOutput>
        {
            new()
            {
                Name = firstProject.Name,
                Color = firstProject.Color,
                TotalTimeInMinutes = 150
            },
            new()
            {
                Name = secondProject.Name,
                Color = secondProject.Color,
                TotalTimeInMinutes = 620
            }
        };

        var timesheets = new List<TimesheetCoreDto>
        {
            await client.TrackFactAsync(userId: netUserId, projectId: firstProjectId, trackedInMinutes: 150),
            await client.TrackFactAsync(userId: netUserId, projectId: secondProjectId, trackedInMinutes: 120),

            await client.TrackFactAsync(userId: javaUserId, projectId: firstProjectId, trackedInMinutes: 100),

            await client.TrackFactAsync(userId: groupManagerId, projectId: secondProjectId, trackedInMinutes: 500)
        };

        await client.AuthAsGroupManager();

        // Act
        var result = await client.GetPieChartProjectsAsync(year: date.Year,
            month: date.Month,
            trackedTimeType: TrackedTimeType.Fact);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResult, opt => opt.WithoutStrictOrdering());

        await client.DeleteTimesheetsUnderCeoRoleAsync(timesheets);
    }
}
