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
public class DeleteTimesheetEndpointIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public DeleteTimesheetEndpointIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task DeleteTimesheetWhenTimesheetRecordDoesNotExistShouldResponseWithNotFoundResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        const int timesheetId = int.MaxValue;

        // Act
        var response = await client.DeleteFactAsHttpMessageAsync(timesheetId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task DeleteTimesheetWhenTimesheetRecordIsCreatedAsOtherUserForOtherProjectShouldResponseWithBadRequestResult(
        bool isBillableProject,
        bool isPublicProject)
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();
        var clientId = _factory.ModsenClientId!.Value;
        var userId = _factory.HeadOfDepartmentUserId!.Value;

        var project = await client.CreateProject(clientId: clientId, isBillable: isBillableProject,
            isPublic: isPublicProject);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId, isTimeBillable: isBillableProject);

        var timesheetRecord = await client.TrackFactAsync(projectId: projectId);
        var factId = timesheetRecord.Id;

        await client.AuthAsEmployee();

        // Act
        var response = await client.DeleteFactAsHttpMessageAsync(factId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task DeleteTimesheetWhenTimesheetRecordIsCreatedAsOtherUserForSameProjectShouldResponseWithBadRequestResult(
        bool isBillableProject,
        bool isPublicProject)
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();

        var clientId = _factory.ModsenClientId!.Value;
        var headId = _factory.HeadOfDepartmentUserId!.Value;
        var employeeId = _factory.EmployeeUserId!.Value;

        var project = await client.CreateProject(clientId: clientId, isBillable: isBillableProject,
            isPublic: isPublicProject);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: employeeId, projectId: projectId, isTimeBillable: isBillableProject);
        await client.CreateProjectAssignment(userId: headId, projectId: projectId, isTimeBillable: isBillableProject);

        var timesheetRecord = await client.TrackFactAsync(projectId: projectId);
        var factId = timesheetRecord.Id;

        await client.AuthAsEmployee();

        // Act
        var response = await client.DeleteFactAsHttpMessageAsync(factId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task DeleteTimesheetWhenTimesheetRecordIsCreatedAsSameUserAndUserIsAssignedToProjectShouldResponseWithOkResult(
        bool isBillableProject,
        bool isPublicProject)
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var clientId = _factory.ModsenClientId!.Value;
        var employeeId = _factory.EmployeeUserId!.Value;

        var project = await client.CreateProject(clientId: clientId, isBillable: isBillableProject,
            isPublic: isPublicProject);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: employeeId, projectId: projectId, isTimeBillable: isBillableProject);

        await client.AuthAsEmployee();

        var timesheetRecord = await client.TrackFactAsync(projectId: projectId);
        var factId = timesheetRecord.Id;

        // Act
        var response = await client.DeleteFactAsHttpMessageAsync(factId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteTimesheetWhenTimesheetRecordIsCreatedAsSameUserAndUserIsNotAssignedToPublicProjectShouldResponseWithOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var clientId = _factory.ModsenClientId!.Value;

        var project = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true);
        var projectId = project!.Id;

        await client.AuthAsEmployee();

        var timesheetRecord = await client.TrackFactAsync(projectId: projectId);
        var factId = timesheetRecord.Id;

        // Act
        var response = await client.DeleteFactAsHttpMessageAsync(factId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task DeleteTimesheetWhenHeadOfDepartmentTriesToDeleteTimesheetOfUserFromOtherDepartmentAndOtherNonPublicProjectShouldResponseWithForbiddenResult(bool isProjectBillable)
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var clientId = _factory.ModsenClientId!.Value;

        var user = await client.CreateEmployee(departmentId: "Mobile");
        var userId = user!.Id;

        var project = await client.CreateProject(clientId: clientId, isBillable: isProjectBillable);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId, isTimeBillable: isProjectBillable);

        var timesheetRecord = await client.TrackFactAsync(userId: userId, projectId: projectId);
        var factId = timesheetRecord.Id;

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.DeleteFactAsHttpMessageAsync(factId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteTimesheetWhenHeadOfDepartmentTriesToDeleteTimesheetOfUserFromOtherDepartmentAndOtherPublicProjectShouldResponseWithForbiddenResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var clientId = _factory.ModsenClientId!.Value;

        var user = await client.CreateEmployee(departmentId: "Mobile");
        var userId = user!.Id;

        var project = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId, isTimeBillable: false);

        var timesheetRecord = await client.TrackFactAsync(userId: userId, projectId: projectId);
        var factId = timesheetRecord.Id;

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.DeleteFactAsHttpMessageAsync(factId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task DeleteTimesheetWhenHeadOfDepartmentTriesToDeleteTimesheetOfUserFromOtherDepartmentAndSameProjectShouldResponseWithForbiddenResult(
        bool isBillableProject,
        bool isPublicProject)
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var clientId = _factory.ModsenClientId!.Value;
        var headId = _factory.HeadOfDepartmentUserId!.Value;

        var user = await client.CreateEmployee(departmentId: "Mobile");
        var userId = user!.Id;

        var project = await client.CreateProject(clientId: clientId, isBillable: isBillableProject,
            isPublic: isPublicProject);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId, isTimeBillable: isBillableProject);
        await client.CreateProjectAssignment(userId: headId, projectId: projectId, isTimeBillable: isBillableProject);

        var timesheetRecord = await client.TrackFactAsync(userId: userId, projectId: projectId);
        var factId = timesheetRecord.Id;

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.DeleteFactAsHttpMessageAsync(factId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task DeleteTimesheetWhenRequestIsValidShouldResponseWithOkResult(
        bool isBillableProject,
        bool isPublicProject)
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = _factory.EmployeeUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;

        var project = await client.CreateProject(clientId: clientId, isBillable: isBillableProject,
            isPublic: isPublicProject);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId, isTimeBillable: isBillableProject);

        var timesheetRecord = await client.TrackFactAsync(userId: userId, projectId: projectId);
        var factId = timesheetRecord.Id;

        // Act
        var response = await client.DeleteFactAsHttpMessageAsync(factId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteTimesheetWhenThereIsAlreadyExistingTimesheetWithBillableHoursOnNonBillableProjectShouldResponseWithOkResultWithNotChangedBillableHours()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var dateTime = DateTime.Now;
        var userId = _factory.EmployeeUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;

        var project = await client.CreateProject(clientId: clientId, isBillable: false);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId, isTimeBillable: false);

        var factTimesheetRecord = await client.TrackFactAsync(userId: userId, projectId: projectId);
        var factId = factTimesheetRecord.Id;

        var billableTimesheetRecord = await client.TrackBillableAsync(userId: userId, projectId: projectId);

        await client.AuthAsEmployee();

        // Act
        var response = await client.DeleteFactAsHttpMessageAsync(factId);

        // Assert
        await client.AuthAsCeo();

        var reportDateRange = DateRangeHelper.GetDateRangeFromMonth(dateTime.Year, dateTime.Month);

        var resultTimesheets = await client.GetTimesheetsAsync(userId: userId,
            dateFrom: reportDateRange.DateFrom,
            dateTo: reportDateRange.DateTo,
            projectId: projectId);

        resultTimesheets.UsersTimesheets.Items.First().Should().Match<UserTimesheetSnapshotDto>(
            timesheet => timesheet.TotalBillableTime == billableTimesheetRecord.BillableTrackedInMinutes &&
                         timesheet.TotalFactTime == 0);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteTimesheetWhenThereIsAlreadyExistingTimesheetWithoutBillableHoursOnNonBillableProjectShouldDeleteTimesheet()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var dateTime = DateTime.Now;
        var userId = _factory.EmployeeUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;

        var project = await client.CreateProject(clientId: clientId, isBillable: false);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId, isTimeBillable: false);

        var factTimesheetRecord = await client.TrackFactAsync(userId: userId, projectId: projectId);
        var factId = factTimesheetRecord.Id;

        await client.AuthAsEmployee();

        // Act
        var response = await client.DeleteFactAsHttpMessageAsync(factId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await client.AuthAsCeo();

        var reportDateRange = DateRangeHelper.GetDateRangeFromMonth(dateTime.Year, dateTime.Month);
        var resultTimesheets = await client.GetTimesheetsAsync(userId: userId, projectId: projectId,
            dateFrom: reportDateRange.DateFrom,
            dateTo: reportDateRange.DateTo);

        resultTimesheets.UsersTimesheets.Items.First(user => user.UserId == userId).Timesheets.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteTimesheetAuthAsEmployeeWhenEmployeeTryingToDeleteSubmittiedTimesheetShouldResponseWithForbidden()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var user = await client.CreateEmployee(departmentId: "Java");
        var userId = user!.Id;

        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId, isTimeBillable: false);

        await client.AuthAs(user.Email);

        var factTimesheetRecord = await client.TrackFactAsync(projectId: projectId, date: date);
        var factId = factTimesheetRecord.Id;

        await client.UpdateTimesheetStatusAsync(userId: userId, month: date.Month, year: date.Year,
            status: TimesheetSubmitStatusType.UnderReview);

        // Act
        var response = await client.DeleteFactAsHttpMessageAsync(factId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
