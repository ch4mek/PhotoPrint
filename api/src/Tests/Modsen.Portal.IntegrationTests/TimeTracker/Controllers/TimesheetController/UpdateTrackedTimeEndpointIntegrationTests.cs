using System.Net;
using System.Text.Json;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.IntegrationTests.TestFramework.Helpers.TimeTracker;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.TimesheetController;

[Collection("TimeTrackerCollection")]
public class UpdateTrackedTimeEndpointIntegrationTests
{

    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public UpdateTrackedTimeEndpointIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-5)]
    public async Task UpdateWhenTrackedInMinutesIsInvalidShouldResponseWithBadRequestResult(int updatedTrackedInMinutes)
    {
        // Arrange
        var client = _factory.CreateClient();
        var userId = _factory.EmployeeUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;
        const string taskName = "TaskName";

        await client.AuthAsCeo();

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        await client.AuthAsEmployee();

        await client.TrackFactAsync(projectId: projectId, name: taskName);

        // Act
        var response = await client.TrackFactAsHttpMessageAsync(projectId: projectId, name: taskName,
            trackedInMinutes: updatedTrackedInMinutes);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task UpdateWhenEmployeeTriesToUpdateOtherUserTimesheetOfSameProjectShouldResponseWithForbiddenResult(
        bool isBillableProject,
        bool isPublicProject)
    {
        // Arrange
        var client = _factory.CreateClient();
        var userHeadId = _factory.HeadOfDepartmentUserId!.Value;
        var employeeUserId = _factory.EmployeeUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;
        const string taskName = "TaskName";

        await client.AuthAsHeadOfDepartment();

        var project = await client.CreateProject(clientId: clientId, isBillable: isBillableProject,
            isPublic: isPublicProject);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userHeadId, projectId: projectId);
        await client.CreateProjectAssignment(userId: employeeUserId, projectId: projectId);

        await client.TrackFactAsync(userId: userHeadId, projectId: projectId, name: taskName);

        await client.AuthAsEmployee();

        // Act
        var response = await client.TrackBillableAsHttpMessageAsync(userId: userHeadId, projectId: projectId,
            name: taskName);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task UpdateWhenEmployeeTriesToUpdateOtherUserTimesheetOfOtherProjectShouldResponseWithForbiddenResult(
        bool isBillableProject,
        bool isPublicProject)
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();
        var userHeadId = _factory.HeadOfDepartmentUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;
        const string taskName = "TaskName";

        var project = await client.CreateProject(clientId: clientId, isBillable: isBillableProject,
            isPublic: isPublicProject);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userHeadId, projectId: projectId);

        await client.TrackFactAsync(projectId: projectId, name: taskName);

        await client.AuthAsEmployee();

        // Act
        var response = await client.TrackBillableAsHttpMessageAsync(userId: userHeadId, projectId: projectId,
            name: taskName);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task UpdateWhenRequestIsValidAndUserIsAssignedToProjectShouldResponseWithOkResult(
        bool isBillableProject,
        bool isPublicProject)
    {
        // Arrange
        var client = _factory.CreateClient();
        var userId = _factory.EmployeeUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;
        const string taskName = "TaskName";

        await client.AuthAsCeo();

        var project = await client.CreateProject(clientId: clientId, isBillable: isBillableProject,
            isPublic: isPublicProject);
        var projectId = project!.Id;

        if (!isPublicProject)
        {
            await client.CreateProjectAssignment(userId: userId, projectId: projectId);
        }

        await client.AuthAsEmployee();

        await client.TrackFactAsync(projectId: projectId, name: taskName);
        await client.TrackFactAsync(projectId: projectId, name: taskName);

        // Act
        var response = await client.TrackFactAsHttpMessageAsync(projectId: projectId, name: taskName);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateWhenRequestIsValidAndUserIsNotAssignedToPublicProjectShouldResponseWithOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        const string taskName = "TaskName";

        var project = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true);
        var projectId = project!.Id;

        await client.AuthAsEmployee();

        await client.TrackFactAsync(projectId: projectId, name: taskName);

        // Act
        var response = await client.TrackFactAsHttpMessageAsync(projectId: projectId, name: taskName);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateWhenHeadOfDepartmentTryToUpdateTimesheetOfUserFromAnotherDepartmentShouldResponseWithForbiddenResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        const string taskName = "TaskName";

        var createdUser = await client.CreateEmployee(departmentId: "Java");
        var userId = createdUser!.Id;

        var project = await client.CreateProject(clientId: clientId, isPublic: true, isBillable: false);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        await client.AuthAs(createdUser.Email);

        await client.TrackFactAsync(userId: userId, projectId: projectId, name: taskName);

        await client.AuthAsHeadOfDepartment();

        // Act
        var responseFact = await client.TrackFactAsHttpMessageAsync(userId: userId, projectId: projectId, name: taskName);

        var response = await client.TrackBillableAsHttpMessageAsync(userId: userId, projectId: projectId,
            name: taskName);

        // Assert
        responseFact.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateWhenThereIsAlreadyExistingTimesheetWithBillableHoursShouldResponseWithOkResultWithChangedBillableHours()
    {
        // Arrange
        var client = _factory.CreateClient();
        var userId = _factory.EmployeeUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;

        var dateTime = DateTime.Now;

        await client.AuthAsCeo();
        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        var input = new UpsertFactDto
        {
            ProjectId = project.Id,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = "JH-6666897",
            FactTrackedInMinutes = 250,
            UserId = userId
        };

        var inputB = new UpsertBillableDto
        {
            ProjectId = project.Id,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = "JH-6666897",
            BillableTrackedInMinutes = 120,
            UserId = userId
        };

        await client.TrackFactAsync(input: input);
        await client.TrackBillableAsync(input: inputB);

        await client.AuthAsEmployee();

        var userInput = new UpsertFactDto
        {
            ProjectId = project.Id,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = "JH-6666897",
            FactTrackedInMinutes = 480,
            UserId = userId
        };

        // Act
        var response = await client.TrackFactAsHttpMessageAsync(userInput);

        // Assert
        await client.AuthAsCeo();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var reportDateRange = DateRangeHelper.GetDateRangeFromMonth(dateTime.Year, dateTime.Month);

        var resultTimesheets = await client.GetTimesheetsAsync(userId: userId, dateFrom: reportDateRange.DateFrom,
            dateTo: reportDateRange.DateTo,
            projectId: projectId);

        resultTimesheets.UsersTimesheets.Items.First().Should().Match<UserTimesheetSnapshotDto>(
            timesheet => timesheet.TotalBillableTime == userInput.FactTrackedInMinutes &&
                         timesheet.TotalFactTime == userInput.FactTrackedInMinutes);
    }

    [Fact]
    public async Task UpdateWhenThereIsAlreadyExistingTimesheetWithBillableHoursOnNonBillableProjectShouldResponseWithOkResultWithNotChangedBillableHours()
    {
        // Arrange
        var client = _factory.CreateClient();
        var userId = _factory.EmployeeUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;

        var dateTime = DateTime.Now;

        await client.AuthAsCeo();
        var project = await client.CreateProject(clientId: clientId, isBillable: false);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId, isTimeBillable: false);

        var input = new UpsertFactDto
        {
            ProjectId = project.Id,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = "JH-6666897",
            FactTrackedInMinutes = 250,
            UserId = userId
        };
        var inputB = new UpsertBillableDto
        {
            ProjectId = project.Id,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = "JH-6666897",
            BillableTrackedInMinutes = 120,
            UserId = userId
        };

        await client.TrackFactAsync(input: input);
        await client.TrackBillableAsync(input: inputB);

        var userInput = new UpsertFactDto
        {
            ProjectId = project.Id,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = "JH-6666897",
            FactTrackedInMinutes = 480,
            UserId = userId
        };

        await client.AuthAsEmployee();

        // Act
        var response = await client.TrackFactAsHttpMessageAsync(userInput);

        // Assert
        await client.AuthAsCeo();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var reportDateRange = DateRangeHelper.GetDateRangeFromMonth(dateTime.Year, dateTime.Month);

        var resultTimesheets = await client.GetTimesheetsAsync(userId: userId, dateFrom: reportDateRange.DateFrom,
            dateTo: reportDateRange.DateTo,
            projectId: projectId);

        resultTimesheets.UsersTimesheets.Items.First().Should().Match<UserTimesheetSnapshotDto>(
            timesheet => timesheet.TotalBillableTime == inputB.BillableTrackedInMinutes &&
                         timesheet.TotalFactTime == userInput.FactTrackedInMinutes);
    }

    [Fact]
    public async Task UpdateAuthAsEmployeeWhenMonthOfTaskIsAlreadySubmittedShouldResponseWithForbidden()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var createdUser = await client.CreateEmployee(departmentId: "Java");
        var userId = createdUser!.Id;

        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId, isTimeBillable: false,
            onProjectFrom: date);

        await client.AuthAs(createdUser.Email);

        await client.TrackFactAsync(projectId: projectId, date: date);

        await client.UpdateTimesheetStatusAsync(userId: userId, month: date.Month, year: date.Year,
            status: TimesheetSubmitStatusType.UnderReview);

        var response = await client.TrackFactAsHttpMessageAsync(projectId: projectId, date: date);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
