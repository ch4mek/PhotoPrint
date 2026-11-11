using System.Globalization;
using System.Net;
using System.Text.Json;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.TimesheetController;

[Collection("TimeTrackerCollection")]
public class PostEndpointIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public PostEndpointIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Theory]
    [MemberData(nameof(GetInvalidCreateTimesheetParams))]
    public async Task PostWhenTaskNameOrTrackedInMinutesIsInvalidShouldResponseWithBadRequestResult(
        string taskName,
        int trackedInMinutes)
    {
        // Arrange
        var client = _factory.CreateClient();
        var clientId = _factory.ModsenClientId!.Value;
        var userId = _factory.EmployeeUserId!.Value;

        await client.AuthAsCeo();
        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        var input = new UpsertFactDto
        {
            ProjectId = project.Id,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = taskName,
            FactTrackedInMinutes = trackedInMinutes
        };

        await client.AuthAsEmployee();

        // Act
        var response = await client.CreateTimesheetFact(input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task PostWhenRequestIsValidAndUserIsAssignedToProjectShouldResponseWithOkResult(
        bool isBillableProject,
        bool isPublicProject)
    {
        // Arrange
        var client = _factory.CreateClient();
        var userId = _factory.HeadOfDepartmentUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;

        await client.AuthAsCeo();

        var project = await client.CreateProject(clientId: clientId, isPublic: isPublicProject,
            isBillable: isBillableProject);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId, isTimeBillable: isBillableProject);

        var input = new UpsertFactDto
        {
            ProjectId = project.Id,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = "TimeTracker",
            FactTrackedInMinutes = 60
        };

        var expectedBody = new TimesheetCoreDto
        {
            CreatedById = userId,
            TaskName = input.TaskName,
            FactTrackedInMinutes = input.FactTrackedInMinutes,
            ReportForDate = input.ReportForDate,
            UserId = userId,
            ProjectId = input.ProjectId
        };

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.TrackFactAsync(input);

        // Assert
        response.Should().BeEquivalentTo(expectedBody, options => options
            .Excluding(x => x.Id)
            .Excluding(x => x.CreatedAtUtc));
        response.Id.Should().NotBe(0);
        response.CreatedAtUtc.Should().NotBe(default);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    public async Task PostWhenUserIsAssignedToProjectAndDoNotHavePermissionToTrackBillableHoursShouldResponseWithBadRequestResult(
        bool isBillableProject,
        bool isPublicProject)
    {
        // Arrange
        var client = _factory.CreateClient();
        var userId = _factory.EmployeeUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;

        await client.AuthAsCeo();

        var project = await client.CreateProject(clientId: clientId, isPublic: isPublicProject,
            isBillable: isBillableProject);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId,
            isTimeBillable: isBillableProject);

        await client.AuthAsEmployee();

        await client.TrackFactAsHttpMessageAsync(new UpsertFactDto
        {
            ProjectId = project.Id,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = "TimeTracker",
            FactTrackedInMinutes = 60,
        });

        // Act
        var response = await client.TrackBillableAsHttpMessageAsync(
            new UpsertBillableDto
            {
                ProjectId = projectId,
                ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
                TaskName = "TimeTracker",
                BillableTrackedInMinutes = 50
            });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    public async Task PostWhenUserIsAssignedToProjectAndHavePermissionToTrackBillableHoursShouldResponseWithOkResult(
        bool isBillableProject,
        bool isPublicProject)
    {
        // Arrange
        var client = _factory.CreateClient();
        var userId = _factory.EmployeeUserId!.Value;

        var clientId = _factory.ModsenClientId!.Value;

        await client.AuthAsCeo();

        var project = await client.CreateProject(clientId: clientId, isPublic: isPublicProject,
            isBillable: isBillableProject);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId, isTimeBillable: isBillableProject);

        var inputFact = new UpsertFactDto
        {
            ProjectId = project.Id,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = "TimeTracker",
            FactTrackedInMinutes = 60,
        };
        var inputBillable = new UpsertBillableDto
        {
            ProjectId = project.Id,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = "TimeTracker",
            BillableTrackedInMinutes = 50
        };

        var expectedBody = new TimesheetCoreDto
        {
            TaskName = inputBillable.TaskName,
            FactTrackedInMinutes = inputFact.FactTrackedInMinutes,
            BillableTrackedInMinutes = inputBillable.BillableTrackedInMinutes,
            ReportForDate = inputBillable.ReportForDate,
            UserId = userId,
            ProjectId = inputBillable.ProjectId
        };

        await client.UpdateUserPermissionAssignments(userId: userId,
                resources: new Dictionary<string, string>
                {
                    { projectId.ToString(CultureInfo.InvariantCulture), project.Name }
                });

        await client.AuthAsEmployee();

        // Act
        await client.CreateTimesheetFact(inputFact);
        var response = await client.TrackBillableAsync(inputBillable);

        // Assert
        response.Should().BeEquivalentTo(expectedBody, options => options
            .Excluding(x => x.Id)
            .Excluding(x => x.CreatedAtUtc)
        );
        response.Id.Should().NotBe(0);
    }

    [Fact]
    public async Task PostWhenRequestIsValidAndUserIsNotAssignedToPublicProjectShouldResponseWithOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        var userId = _factory.HeadOfDepartmentUserId!.Value;

        var clientId = _factory.ModsenClientId!.Value;

        await client.AuthAsCeo();

        var project = await client.CreateProject(clientId: clientId, isPublic: true, isBillable: false);
        var projectId = project!.Id;

        var inputFact = new UpsertFactDto
        {
            ProjectId = projectId,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = "TimeTracker",
            FactTrackedInMinutes = 60
        };

        var expectedBody = new TimesheetCoreDto
        {
            CreatedById = userId,
            TaskName = inputFact.TaskName,
            FactTrackedInMinutes = inputFact.FactTrackedInMinutes,
            ReportForDate = inputFact.ReportForDate,
            UserId = userId,
            ProjectId = inputFact.ProjectId
        };

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.TrackFactAsync(inputFact);

        // Assert
        response.Should().BeEquivalentTo(expectedBody, options => options
            .Excluding(x => x.Id)
            .Excluding(x => x.CreatedAtUtc));
        response.Id.Should().NotBe(0);
        response.CreatedAtUtc.Should().NotBe(default);
    }

    [Fact]
    public async Task PostWhenProjectDoesNotExistShouldResponseWithNotFoundResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var input = new UpsertFactDto
        {
            ProjectId = int.MaxValue,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = "TimeTracker",
            FactTrackedInMinutes = 60,
        };

        // Act
        var response = await client.TrackFactAsHttpMessageAsync(input);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task PostWhenUserIsNotAssignedToNonPublicProjectShouldResponseWithBadRequestResult(bool isProjectBillable)
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var clientId = _factory.ModsenClientId!.Value;

        var project = await client.CreateProject(clientId: clientId, isBillable: isProjectBillable);
        var projectId = project!.Id;

        var inputFact = new UpsertFactDto
        {
            ProjectId = projectId,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = "TimeTracker",
            FactTrackedInMinutes = 60
        };

        await client.AuthAsEmployee();

        // Act
        var response = await client.TrackFactAsHttpMessageAsync(inputFact);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostWhenAuthAsCeoAndPostDoubleValuesWithPrivateBillableProjectShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var user = await client.CreateEmployee(departmentId: "Mobile");
        var userId = user!.Id;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        var inputFact = new UpsertFactDto
        {
            ProjectId = projectId,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = "TimeTracker",
            FactTrackedInMinutes = 60,
            UserId = user.Id
        };

        var inputBillable = new UpsertBillableDto
        {
            ProjectId = projectId,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = "TimeTracker",
            BillableTrackedInMinutes = 60,
            UserId = user.Id
        };

        // Act
        var responseF = await client.TrackFactAsHttpMessageAsync(inputFact);
        var responseB = await client.TrackBillableAsHttpMessageAsync(inputBillable);

        // Assert
        responseF.StatusCode.Should().Be(HttpStatusCode.OK);
        responseB.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostWhenReportForDateIsInvalidShouldResponseWithBadRequestResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var userId = _factory.EmployeeUserId!.Value;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        var input = new UpsertFactDto
        {
            ProjectId = project.Id,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            TaskName = "TimeTracker",
            FactTrackedInMinutes = 60
        };

        await client.AuthAsEmployee();

        // Act
        var response = await client.TrackFactAsHttpMessageAsync(input: input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostWhenReportForDateHasMonthLessThanCurrentAndYearMoreThanCurrentShouldResponseWithOk()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var userId = _factory.EmployeeUserId!.Value;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        var currentDateTime = DateOnly.FromDateTime(DateTime.UtcNow);

        var timesheetDate = currentDateTime.AddMonths(-1).AddYears(1);

        var input = new UpsertFactDto
        {
            ProjectId = project.Id,
            ReportForDate = timesheetDate,
            TaskName = "TimeTracker",
            FactTrackedInMinutes = 60
        };

        await client.AuthAsEmployee();

        // Act
        var response = await client.TrackFactAsHttpMessageAsync(input: input);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostWhenHeadOfDepartmentTriesToCreateTrackTimeForUserFromOtherDepartmentAndOtherBillableProjectShouldResponseWithForbiddenResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var user = await client.CreateEmployee(departmentId: "Mobile");
        var userId = user!.Id;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        await client.AuthAsHeadOfDepartment();

        var input = new UpsertFactDto
        {
            ProjectId = project.Id,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-40)),
            TaskName = "AB-746",
            FactTrackedInMinutes = 240,
            UserId = user.Id
        };

        // Act
        var response = await client.TrackFactAsHttpMessageAsync(input: input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task PostWhenHeadOfDepartmentTriesToCreateTrackTimeForUserFromOtherDepartmentAndOtherPublicProjectShouldResponseWithForbiddenResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var user = await client.CreateEmployee(departmentId: "Mobile");
        var userId = user!.Id;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        await client.AuthAsHeadOfDepartment();

        var input = new UpsertFactDto
        {
            ProjectId = projectId,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-40)),
            TaskName = "AB-746",
            FactTrackedInMinutes = 240,
            UserId = userId
        };

        // Act
        var response = await client.TrackFactAsHttpMessageAsync(input: input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task PostWhenHeadOfDepartmentTriesToTrackTimeForUserFromOtherDepartmentAndSameProjectShouldResponseWithForbiddenResult(
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

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);
        await client.CreateProjectAssignment(userId: headId, projectId: projectId);

        await client.AuthAsHeadOfDepartment();

        var input = new UpsertFactDto
        {
            ProjectId = projectId,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-40)),
            TaskName = "BB-853",
            FactTrackedInMinutes = 240,
            UserId = userId
        };

        // Act
        var response = await client.TrackFactAsHttpMessageAsync(input: input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task PostWhenRequestIsValidAndUserIsNotAssignedToNonPublicProjectShouldResponseWithBadRequestResult(bool isProjectBillable)
    {
        // Arrange
        var client = _factory.CreateClient();
        var clientId = _factory.ModsenClientId!.Value;
        var userId = _factory.EmployeeUserId!.Value;

        await client.AuthAsCeo();

        var project = await client.CreateProject(clientId: clientId, isBillable: isProjectBillable,
            isPublic: false);
        var projectId = project!.Id;

        var input = new UpsertFactDto
        {
            ProjectId = projectId,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = "JH-842",
            FactTrackedInMinutes = 480,
            UserId = userId
        };

        // Act
        var response = await client.TrackFactAsHttpMessageAsync(input: input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostWhenThereIsAlreadyExistingTimesheetWithoutFactHoursShouldResponseWithExpectedResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        var userId = _factory.EmployeeUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;

        await client.AuthAsCeo();

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        var inputB = new UpsertBillableDto
        {
            ProjectId = projectId,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = "JH-6666897",
            BillableTrackedInMinutes = 120,
            UserId = userId
        };

        var timesheet = await client.TrackBillableAsync(input: inputB);

        var inputFact = new UpsertFactDto
        {
            ProjectId = projectId,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = "JH-6666897",
            FactTrackedInMinutes = 480,
            UserId = userId
        };

        var expectedTimesheet = new TimesheetCoreDto
        {
            Id = timesheet.Id,
            TaskName = timesheet.TaskName,
            FactTrackedInMinutes = inputFact.FactTrackedInMinutes,
            BillableTrackedInMinutes = inputFact.FactTrackedInMinutes,
            ReportForDate = timesheet.ReportForDate,
            UserId = timesheet.UserId,
            ProjectId = timesheet.ProjectId
        };

        await client.AuthAsEmployee();

        // Act
        var response = await client.TrackFactAsync(input: inputFact);

        // Assert
        response.Should().BeEquivalentTo(expectedTimesheet);
    }

    public static IEnumerable<object[]> GetInvalidCreateTimesheetParams()
    {
        yield return ["Test", -1];
        yield return ["Test task", -5];
    }

    [Fact]
    public async Task PostWhenReportDateOutsideAssignIntervalResponseWithBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = _factory.EmployeeUserId!.Value;

        var project = await client.CreateProject(clientId: 2, isBillable: false);
        var projectId = project!.Id;

        var reportForDate = DateOnly.FromDateTime(DateTime.UtcNow);

        await client.CreateProjectAssignment(userId: userId, projectId: projectId,
            onProjectFrom: reportForDate.AddDays(5), isTimeBillable: false);

        var input = new UpsertFactDto
        {
            UserId = userId,
            FactTrackedInMinutes = 480,
            ProjectId = projectId,
            ReportForDate = reportForDate,
            TaskName = Guid.CreateVersion7().ToAlphabetic()
        };

        // Act
        var response = await client.TrackFactAsHttpMessageAsync(input: input);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task PostWhenReportDateInsideAssignIntervalResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = _factory.EmployeeUserId!.Value;

        var project = await client.CreateProject(clientId: 2, isBillable: false);
        var projectId = project!.Id;

        var reportForDate = DateOnly.FromDateTime(DateTime.UtcNow);

        await client.CreateProjectAssignment(userId: userId, projectId: projectId,
            onProjectFrom: reportForDate.AddDays(-5), isTimeBillable: false);

        var inputFact = new UpsertFactDto
        {
            UserId = userId,
            FactTrackedInMinutes = 480,
            ProjectId = project.Id,
            ReportForDate = reportForDate,
            TaskName = Guid.CreateVersion7().ToAlphabetic()
        };

        // Act
        var response = await client.TrackFactAsync(inputFact);

        // Assert
        response.ProjectId.Should().Be(project.Id);
        response.UserId.Should().Be(userId);
        response.ReportForDate.Should().Be(reportForDate);
    }

    [Fact]
    public async Task PostAuthAsEmployeeWhenMonthIsAlreadySubmittedShouldResponseWithForbidden()
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

        await client.CreateProjectAssignment(userId: userId, projectId: projectId,
            onProjectFrom: date, isTimeBillable: false);

        await client.AuthAs(createdUser.Email);

        await client.UpdateTimesheetStatusAsync(userId: userId, month: date.Month, year: date.Year,
            status: TimesheetSubmitStatusType.UnderReview);

        var input = new UpsertFactDto
        {
            ProjectId = projectId,
            ReportForDate = date,
            TaskName = "Task #745",
            FactTrackedInMinutes = 60
        };

        // Act
        var response = await client.TrackFactAsHttpMessageAsync(input: input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
