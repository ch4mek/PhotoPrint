using System.Net;
using System.Text.Json;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.TimesheetController;

[Collection("TimeTrackerCollection")]
public class UpdateTaskNameEndpointIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public UpdateTaskNameEndpointIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task UpdateTaskNameWhenTimesheetRecordDoesNotExistShouldResponseWithNotFoundResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var input = new UpdateTaskNameDto
        {
            UserId = 2,
            ProjectId  = _factory.IdleProjectId.Value,
            EndDate = new DateOnly(2022, 5, 2),
            StartDate = new DateOnly(2022, 3, 22),
            OldTaskName = "Test",
            TaskName = "Some other task"
        };

        // Act
        var response = await client.UpdateTaskNameAsHttpMessageAsync(input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task UpdateTaskNameWhenEmployeeTryToUpdateOtherUserTaskNameOfSameProjectShouldResponseWithForbiddenResult(
        bool isBillableProject,
        bool isPublicProject)
    {
        // Arrange
        var client = _factory.CreateClient();
        var userId = _factory.EmployeeUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;
        var headId = _factory.HeadOfDepartmentUserId!.Value;

        await client.AuthAsHeadOfDepartment();
        var project = await client.CreateProject(clientId: clientId, isBillable: isBillableProject,
            isPublic: isPublicProject);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);
        await client.CreateProjectAssignment(userId: headId, projectId: projectId);

        var timesheetRecord = await client.TrackFactAsync(projectId: projectId);

        await client.AuthAsEmployee();

        var input = new UpdateTaskNameDto
        {
            UserId = timesheetRecord.UserId,
            ProjectId = timesheetRecord.ProjectId,
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            StartDate = new DateOnly(2022, 5, 2),
            OldTaskName = timesheetRecord.TaskName!,
            TaskName = "Changed TaskName"
        };

        // Act
        var response = await client.UpdateTaskNameAsHttpMessageAsync(input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task UpdateTaskNameWhenEmployeeTryToUpdateOtherUserTimesheetOfOtherProjectShouldResponseWithForbiddenResult(
        bool isBillableProject,
        bool isPublicProject)
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();
        var clientId = _factory.ModsenClientId!.Value;
        var headId = _factory.HeadOfDepartmentUserId!.Value;

        var project = await client.CreateProject(clientId: clientId, isBillable: isBillableProject,
            isPublic: isPublicProject);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: headId, projectId: projectId);

        var timesheetRecord = await client.TrackFactAsync(projectId: projectId);

        await client.AuthAsEmployee();

        var input = new UpdateTaskNameDto
        {
            UserId = timesheetRecord.UserId,
            ProjectId = timesheetRecord.ProjectId,
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            StartDate = new DateOnly(2022, 5, 2),
            OldTaskName = timesheetRecord.TaskName!,
            TaskName = "TT-999"
        };

        // Act
        var response = await client.UpdateTaskNameAsHttpMessageAsync(input: input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData(false, false, 0)]
    [InlineData(true, false, 60)]
    [InlineData(false, true, 0)]
    public async Task UpdateTaskNameWhenRequestIsValidAndUserIsAssignedToProjectShouldResponseWithOkResult(
        bool isBillableProject,
        bool isPublicProject,
        int billableTrackedInMinutes)
    {
        // Arrange
        var client = _factory.CreateClient();
        var userId = _factory.EmployeeUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;

        await client.AuthAsCeo();
        var project = await client.CreateProject(clientId: clientId, isBillable: isBillableProject,
            isPublic: isPublicProject);
        var projectId = project!.Id;

        if (!isPublicProject)
        {
            await client.CreateProjectAssignment(userId: userId, projectId: projectId);
        }

        await client.AuthAsEmployee();

        var timesheetRecord = await client.TrackFactAsync(projectId: projectId);

        var input = new UpdateTaskNameDto
        {
            UserId = timesheetRecord.UserId,
            ProjectId = timesheetRecord.ProjectId,
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            StartDate = new DateOnly(2022, 5, 2),
            OldTaskName = timesheetRecord.TaskName!,
            TaskName = "TT-964 - add functionality"
        };

        // Act
        var response = await client.UpdateTaskNameAsHttpMessageAsync(input: input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateTaskNameWhenRequestIsValidAndUserIsNotAssignedToPublicProjectShouldResponseWithOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var userId = _factory.EmployeeUserId!.Value;

        var project = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        await client.AuthAsEmployee();

        var timesheetRecord = await client.TrackFactAsync(projectId: projectId);

        var input = new UpdateTaskNameDto
        {
            UserId = timesheetRecord.UserId,
            ProjectId = timesheetRecord.ProjectId,
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            StartDate = new DateOnly(2022, 5, 2),
            OldTaskName = timesheetRecord.TaskName!,
            TaskName = "TT-964 - add functionality"
        };

        // Act
        var response = await client.UpdateTaskNameAsHttpMessageAsync(input: input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateTaskNameWhenHeadOfDepartmentTriesToUpdateTimesheetForUserFromOtherDepartmentAndOtherNonPublicProjectShouldResponseWithForbiddenResult(
        bool isProjectBillable)
    {
        // Arrange
        var client = _factory.CreateClient();
        var clientId = _factory.ModsenClientId!.Value;

        await client.AuthAsCeo();
        var user = await client.CreateEmployee(departmentId: "Mobile");
        var userId = user!.Id;

        var project = await client.CreateProject(clientId: clientId, isBillable: isProjectBillable);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        var timesheetRecord = await client.TrackFactAsync(userId: userId, projectId: projectId);

        await client.AuthAsHeadOfDepartment();

        var input = new UpdateTaskNameDto
        {
            UserId = timesheetRecord.UserId,
            ProjectId = timesheetRecord.ProjectId,
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            StartDate = new DateOnly(2022, 5, 2),
            OldTaskName = timesheetRecord.TaskName!,
            TaskName = Guid.CreateVersion7().ToString()
        };

        // Act
        var response = await client.UpdateTaskNameAsHttpMessageAsync(input: input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateTaskNameWhenHeadOfDepartmentTriesToUpdateTimesheetForUserFromOtherDepartmentAndOtherPublicProjectShouldResponseWithBForbiddenResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        var clientId = _factory.ModsenClientId!.Value;

        await client.AuthAsCeo();
        var user = await client.CreateEmployee(departmentId: "Mobile");
        var userId = user!.Id;

        var project = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        var timesheetRecord = await client.TrackFactAsync(userId: userId, projectId: projectId);

        await client.AuthAsHeadOfDepartment();

        var input = new UpdateTaskNameDto
        {
            UserId = timesheetRecord.UserId,
            ProjectId = timesheetRecord.ProjectId,
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            StartDate = new DateOnly(2022, 5, 2),
            OldTaskName = timesheetRecord.TaskName!,
            TaskName = Guid.CreateVersion7().ToString()
        };

        // Act
        var response = await client.UpdateTaskNameAsHttpMessageAsync(input: input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task UpdateTaskNameWhenHeadOfDepartmentTriesToUpdateTimesheetForUserFromOtherDepartmentAndSameProjectShouldResponseWithForbiddenResult(
        bool isBillableProject,
        bool isPublicProject)
    {
        // Arrange
        var client = _factory.CreateClient();
        var clientId = _factory.ModsenClientId!.Value;
        var headId = _factory.HeadOfDepartmentUserId!.Value;

        await client.AuthAsCeo();
        var user = await client.CreateEmployee(departmentId: "Mobile");
        var userId = user!.Id;

        var project = await client.CreateProject(clientId: clientId, isBillable: isBillableProject,
            isPublic: isPublicProject);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);
        await client.CreateProjectAssignment(userId: headId, projectId: projectId);

        var timesheetRecord = await client.TrackFactAsync(userId: userId, projectId: projectId);

        await client.AuthAsHeadOfDepartment();

        var input = new UpdateTaskNameDto
        {
            UserId = timesheetRecord.UserId,
            ProjectId = timesheetRecord.ProjectId,
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            StartDate = new DateOnly(2022, 5, 2),
            OldTaskName = timesheetRecord.TaskName!,
            TaskName = Guid.CreateVersion7().ToString()
        };

        // Act
        var response = await client.UpdateTaskNameAsHttpMessageAsync(input: input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task UpdateTaskNameWhenRequestIsValidShouldResponseWithOkResult(
        bool isBillableProject,
        bool isPublicProject)
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var userId = _factory.EmployeeUserId!.Value;

        var project = await client.CreateProject(clientId: clientId, isBillable: isBillableProject,
            isPublic: isPublicProject);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        var timesheetRecord = await client.TrackFactAsync(userId: userId, projectId: projectId);

        var input = new UpdateTaskNameDto
        {
            UserId = timesheetRecord.UserId,
            ProjectId = timesheetRecord.ProjectId,
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            StartDate = new DateOnly(2022, 5, 2),
            OldTaskName = timesheetRecord.TaskName!,
            TaskName = Guid.CreateVersion7().ToString()
        };

        // Act
        var response = await client.UpdateTaskNameAsHttpMessageAsync(input: input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateTaskNameAuthAsEmployeeWhenMonthOfTaskIsAlreadySubmittedShouldResponseWithForbidden()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var clientId = _factory.ModsenClientId!.Value;
        var user = await client.CreateEmployee(departmentId: "Mobile");
        var userId = user!.Id;

        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId, onProjectFrom: date,
            isTimeBillable: false);

        await client.AuthAs(user.Email);

        var timesheetRecord = await client.TrackFactAsync(projectId: projectId);

        await client.UpdateTimesheetStatusAsync(userId: userId, year: date.Year, month: date.Month,
            status: TimesheetSubmitStatusType.UnderReview);

        // Act
        var input = new UpdateTaskNameDto
        {
            UserId = timesheetRecord.UserId,
            ProjectId = timesheetRecord.ProjectId,
            EndDate = date.AddDays(1),
            StartDate = date,
            OldTaskName = timesheetRecord.TaskName!,
            TaskName = Guid.CreateVersion7().ToString()
        };

        // Act
        var response = await client.UpdateTaskNameAsHttpMessageAsync(input: input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
