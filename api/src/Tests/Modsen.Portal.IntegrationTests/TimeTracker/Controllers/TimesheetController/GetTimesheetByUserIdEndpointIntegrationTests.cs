using System.Globalization;
using System.Net;
using System.Text.Json;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.TimesheetController;

[Collection("TimeTrackerCollection")]
public class GetTimesheetByUserIdEndpointIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetTimesheetByUserIdEndpointIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetTimesheetsByUserIdWhenAuthedAsEmployeeAndTryToGetNotOwnTimesheetsShouldResponseWithForbiddenResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var userId = _factory.EmployeeUserId!.Value;

        // Act
        var response = await client.GetTimesheetByUserIdAsHttpMessageAsync(userId: ++userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetTimesheetsByUserIdWhenAuthedAsEmployeeAndTryToGetOwnTimesheetsShouldResponseWithOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var userId = _factory.EmployeeUserId!.Value;

        // Act
        var response = await client.GetTimesheetByUserIdAsHttpMessageAsync(userId: userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTimesheetsByUserIdWhenAuthedEmployeeIsFromPmoDepartmentShouldResponseWithOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var user = await client.CreateEmployee(departmentId: "PMO");
        var firstManagerId = user!.Id;

        var createdEmployee = await client.CreateEmployee(departmentId: "PMO");
        var secondManagerId = createdEmployee!.Id;

        var createdPrivateProject = await client.CreateProject(clientId: clientId, managerId: firstManagerId);
        var projectId = createdPrivateProject!.Id;

        await client.CreateProjectAssignment(userId: secondManagerId, projectId: projectId);

        await client.AuthAs(user.Email);

        // Act
        var response = await client.GetTimesheetByUserIdAsHttpMessageAsync(userId: secondManagerId,
            onlyManagedProjects: true);

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTimesheetsByUserIdWhenUserDoesNotHaveFactHoursAndPermissionToTrackBillableHoursShouldResponseWithEmptyTimesheetList()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var clientId = _factory.ModsenClientId!.Value;
        var pmoHeadEmail = _factory.PmoHeadOfDepartmentEmail!;
        var billableMinutesTotrack = 60;

        var user = await client.CreateEmployee(departmentId: "PMO");
        var userId = user!.Id;

        await client.AuthAsCeo();

        var createdProject = await client.CreateProject(clientId: clientId);
        var projectId = createdProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        await client.AuthAs(pmoHeadEmail);

        await client.TrackBillableAsync(userId: userId, projectId: projectId, trackedInMinutes: billableMinutesTotrack);

        // Act
        await client.AuthAs(user.Email);

        var response = await client.GetTimesheetByUserIdAsync(userId: userId);

        // Assert
        response.PageableListOfItems.Items.Sum(x=>x.BillableTrackedInMinutes)
            .Should().BeGreaterThanOrEqualTo(billableMinutesTotrack);
    }

    [Fact]
    public async Task GetTimesheetsByUserIdWhenUserDoesNotHaveFactHoursAndHasPermissionToTrackBillableHoursOnlyOnAnotherProjectShouldResponseWithEmptyTimesheetList()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var pmoHeadEmail = _factory.PmoHeadOfDepartmentEmail!;
        var billableMinutesTotrack = 60;

        var user = await client.CreateEmployee(departmentId: "PMO");
        var userId = user!.Id;

        var createdProject = await client.CreateProject(clientId: clientId);
        var firstProjectId = createdProject!.Id;

        var anotherCreatedProject = await client.CreateProject(clientId: clientId);
        var secondProjectId = anotherCreatedProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: firstProjectId);
        await client.CreateProjectAssignment(userId: userId, projectId: secondProjectId);

        await client.UpdateUserPermissionAssignments(userId: userId,
            resources: new Dictionary<string, string>
            {
                { secondProjectId.ToString(CultureInfo.InvariantCulture), anotherCreatedProject.Name}
            });

        await client.AuthAs(pmoHeadEmail);

        await client.TrackBillableAsync(userId: userId, projectId: firstProjectId, trackedInMinutes: billableMinutesTotrack);

        await client.AuthAs(user.Email);

        // Act
        var response = await client.GetTimesheetByUserIdAsync(userId: userId);

        // Assert
        response.PageableListOfItems.Items.Sum(x=>x.BillableTrackedInMinutes)
            .Should().BeGreaterThanOrEqualTo(billableMinutesTotrack);
    }

    [Fact]
    public async Task GetTimesheetsByUserIdWhenAuthedAsSystemAdministratorAndTryToGetNotOwnTimesheetsShouldResponseWithForbiddenResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsAdmin();

        var userId = _factory.AdminUserId!.Value;

        // Act
        var response = await client.GetTimesheetByUserIdAsHttpMessageAsync(userId: ++userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetTimesheetsByUserIdWhenAuthedAsSystemAdministratorAndTryToGetOwnTimesheetsShouldResponseWithOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsAdmin();

        var userId = _factory.AdminUserId!.Value;

        // Act
        var response = await client.GetTimesheetByUserIdAsHttpMessageAsync(userId: userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTimesheetsByUserIdWhenHeadOfDepartmentTriesToGetTimesheetsOfOtherDepartmentShouldResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var user = await client.CreateEmployee(departmentId: "Mobile");
        var userId = user!.Id;

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.GetTimesheetByUserIdAsHttpMessageAsync(userId: userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetTimesheetsByUserIdWhenHeadOfDepartmentTriesToGetTimesheetsOfUserFromAnotherDepartmentAndUserHasProjectWhereManagerIsHeadShouldReturnOnlyTimesheetsForThisProject()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var pmoHeadEmail = _factory.PmoHeadOfDepartmentEmail!;
        var pmoHeadId = _factory.PmoHeadOfDepartmentUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;

        var user = await client.CreateEmployee(departmentId: "Mobile");
        var userId = user!.Id;

        var projectWithManager = await client.CreateProject(clientId: clientId, managerId: pmoHeadId);
        var projectWithManagerId = projectWithManager!.Id;

        var projectWithoutManager = await client.CreateProject(clientId: clientId);
        var projectWithoutManagerId = projectWithoutManager!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectWithManagerId);
        await client.CreateProjectAssignment(userId: userId, projectId: projectWithoutManagerId);

        await client.TrackFactAsync(userId: userId, projectId: projectWithoutManagerId);

        var timesheetForProjectWithManager = await client.TrackBillableAsync(userId: userId, projectId: projectWithManagerId);
        await client.TrackBillableAsync(userId: userId, projectId: projectWithoutManagerId);

        var expectedResult = new UserTimesheetsWithBlockedDaysDto(
            PageableListOfItemsExtensions.ToPageable(
                    [
                        new UserTimesheetDto
                        {
                            Id = timesheetForProjectWithManager.Id,
                            ProjectId = projectWithManager.Id,
                            ProjectName = projectWithManager.Name,
                            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
                            TaskName = timesheetForProjectWithManager.TaskName!,
                            FactTrackedInMinutes = timesheetForProjectWithManager.FactTrackedInMinutes,
                            BillableTrackedInMinutes = timesheetForProjectWithManager.BillableTrackedInMinutes,
                            IsProjectActive = true
                        }
                    ]
                )
                .WithPaging(1, 100)
                .WithTotal(1), []
        );

        await client.AuthAs(pmoHeadEmail);

        // Act
        var response = await client.GetTimesheetByUserIdAsync(userId: userId);

        // Assert
        response.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task GetTimesheetsByUserIdAuthedAsGroupManagerShouldReturnOnlyTimesheetsOfTheResourceGroupEmployees()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var createdUserFromGroup = await client.CreateEmployee(resourceGroupId: 1);
        var userId = createdUserFromGroup!.Id;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        var timesheet = await client.TrackBillableAsync(userId: userId, projectId: projectId);

        var expectedResult = new UserTimesheetsWithBlockedDaysDto(
            PageableListOfItemsExtensions.ToPageable(
                    [
                        new UserTimesheetDto
                        {
                            Id = timesheet.Id,
                            ProjectId = project.Id,
                            ProjectName = project.Name,
                            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
                            TaskName = timesheet.TaskName!,
                            FactTrackedInMinutes = timesheet.FactTrackedInMinutes,
                            BillableTrackedInMinutes = timesheet.BillableTrackedInMinutes,
                            IsProjectActive = true
                        }
                    ]
                )
                .WithPaging(1, 100)
                .WithTotal(1),
            []
        );

        await client.AuthAsGroupManager();

        // Act

        var response = await client.GetTimesheetByUserIdAsync(userId: userId);

        // Assert
        response.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task GetTimesheetsByUserIdWhenHeadOfDepartmentIsManagerOfUserProjectsAndTryToGetUserTimesheetsShouldReturnOnlyTimesheetsFromManagedProject()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var pmoHeadEmail = _factory.PmoHeadOfDepartmentEmail!;
        var pmoHeadId = _factory.PmoHeadOfDepartmentUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;

        var user = await client.CreateEmployee(departmentId: "Mobile");
        var userId = user!.Id;

        var projectForHeadToManage = await client.CreateProject(clientId: clientId);
        var projectWithManagerId = projectForHeadToManage!.Id;

        var projectWithoutManager = await client.CreateProject(clientId: clientId);
        var projectWithoutManagerId = projectWithoutManager!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectWithManagerId);
        await client.CreateProjectAssignment(userId: userId, projectId: projectWithoutManagerId);

        var updateProjectInput = new UpdateProjectInput
        {
            Id = projectForHeadToManage.Id,
            Name = projectForHeadToManage.Name,
            ClientId = projectForHeadToManage.ClientId,
            IsBillable = projectForHeadToManage.IsBillable,
            IsPublic = projectForHeadToManage.IsPublic,
            ManagerId = pmoHeadId,
            Color = projectForHeadToManage.Color,
        };

        await client.UpdateProjectAsync(updateProjectInput);

        await client.AuthAs(user.Email);

        await client.TrackFactAsync(projectId: projectWithoutManagerId);
        var createdTimesheet = await client.TrackFactAsync(projectId: projectWithManagerId);

        // Act
        await client.AuthAs(pmoHeadEmail);

        var timesheets = await client.GetTimesheetByUserIdAsync(userId: userId,
            projectId: projectWithoutManagerId);

        // Assert
        var timesheetsIdsList = timesheets.PageableListOfItems.Items.Select(x => x.Id).ToList();
        timesheetsIdsList.Should().BeEquivalentTo(new List<int> { createdTimesheet.Id });

    }

    [Fact]
    public async Task GetTimesheetsByUserIdWhenHeadOfDepartmentIsManagerOfProjectWhereUserHasTrackedTimeShouldReturnOnlyTimesheetsFromManagedProject()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var pmoHeadEmail = _factory.PmoHeadOfDepartmentEmail!;
        var pmoHeadId = _factory.PmoHeadOfDepartmentUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;

        var employee = await client.CreateEmployee();
        var userId = employee!.Id;

        var projectForHeadToManage = await client.CreateProject(clientId: clientId);
        var projectWithManagerId = projectForHeadToManage!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectWithManagerId);

        var projectWithoutManager = await client.CreateProject(clientId: clientId);
        var projectWithoutManagerId = projectWithoutManager!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectWithoutManagerId);

        var updateProjectInput = new UpdateProjectInput
        {
            Id = projectForHeadToManage.Id,
            Name = projectForHeadToManage.Name,
            ClientId = projectForHeadToManage.ClientId,
            IsBillable = projectForHeadToManage.IsBillable,
            IsPublic = projectForHeadToManage.IsPublic,
            ManagerId = pmoHeadId,
            Color = projectForHeadToManage.Color,
        };

        await client.UpdateProjectAsync(updateProjectInput);

        await client.AuthAs(employee.Email);

        await client.TrackFactAsync(projectId: projectWithoutManagerId);
        var createdTimesheet = await client.TrackFactAsync(projectId: projectWithManagerId);

        //Act
        await client.AuthAs(pmoHeadEmail);

        var timesheets = await client.GetTimesheetByUserIdAsync(userId: userId,
            onlyManagedProjects: true);

        // Assert
        var timesheetsIdsList = timesheets.PageableListOfItems.Items.Select(x => x.Id).ToList();
        timesheetsIdsList.Should().BeEquivalentTo(new List<int> { createdTimesheet.Id });
    }

    [Fact]
    public async Task GetTimesheetsByUserIdWhenHeadOfDepartmentTriesToGetTimesheetsOfSameDepartmentShouldResponseWithOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var employee = await client.CreateEmployee();
        var userId = employee!.Id;

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.GetTimesheetByUserIdAsHttpMessageAsync(userId: userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTimesheetsByUserIdWhenAuthedAsCEOAndTryToGetNotOwnTimesheetsShouldResponseWithOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = _factory.EmployeeUserId!.Value;

        // Act
        var response = await client.GetTimesheetByUserIdAsHttpMessageAsync(userId: ++userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTimesheetsByUserIdWhenAuthedAsCTOAndTryToGetNotOwnTimesheetsShouldResponseWithOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var userId = _factory.EmployeeUserId!.Value;

        // Act
        var response = await client.GetTimesheetByUserIdAsHttpMessageAsync(userId: ++userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTimesheetsByUserIdWhenRequestIsValidShouldResponseWithOkResultAndFilteredTimesheets()
    {
        // Arrange
        var client = _factory.CreateClient();
        var userId = _factory.EmployeeUserId!.Value;
        var timesheetDate = new DateOnly(2022, 07, 17);
        var clientId = _factory.ModsenClientId!.Value;

        await client.AuthAsCeo();
        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId,
            onProjectFrom: timesheetDate.AddDays(-40));

        await client.CreateTimesheetToTimesheetManagerAsHttpMessageAsync(userId: userId, projectId: projectId,
            date: timesheetDate.AddDays(-40));

        var timesheetRecord = await client.TrackBillableAsync(userId: userId, projectId: projectId,
            date: timesheetDate);

        var expectedBody = new UserTimesheetsWithBlockedDaysDto(
            PageableListOfItemsExtensions.ToPageable(
                    [
                        new UserTimesheetDto
                        {
                            Id = timesheetRecord.Id,
                            ReportForDate = timesheetRecord.ReportForDate,
                            FactTrackedInMinutes = timesheetRecord.FactTrackedInMinutes,
                            BillableTrackedInMinutes = timesheetRecord.BillableTrackedInMinutes,
                            ProjectId = project.Id,
                            ProjectName = project.Name,
                            TaskName = timesheetRecord.TaskName!,
                            IsProjectActive = true
                        }
                    ]
                )
                .WithPaging(1, 100)
                .WithTotal(1), []
        );

        // Act
        var filter =
            $"( reportYear eq {timesheetDate.Year} and reportMonthOfYear eq {timesheetDate.Month} and clientId eq {clientId} and projectId eq {project.Id} )";
        var response = await client.GetTimesheetByUserIdAsync(userId: userId, filter: filter);

        // Assert
        response.Should().BeEquivalentTo(expectedBody);
    }

    [Fact]
    public async Task GetTimesheetsByUserIdWithPostMethodWhenRequestIsValidShouldResponseWithOkResultAndFilteredTimesheets()
    {
        // Arrange
        var client = _factory.CreateClient();
        var userId = _factory.EmployeeUserId!.Value;
        var timesheetDateTime = new DateOnly(2022, 07, 17);
        var clientId = _factory.ModsenClientId!.Value;

        await client.AuthAsCeo();
        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId,
            onProjectFrom: timesheetDateTime.AddDays(-40));

        await client.CreateTimesheetToTimesheetManagerAsHttpMessageAsync(userId: userId, projectId: projectId,
            date: timesheetDateTime.AddDays(-40));

        var timesheetRecord = await client.TrackBillableAsync(userId: userId, projectId: projectId,
            date: timesheetDateTime);

        var expectedBody = new UserTimesheetsWithBlockedDaysDto(
            PageableListOfItemsExtensions.ToPageable(
                    [
                        new UserTimesheetDto
                        {
                            Id = timesheetRecord.Id,
                            ReportForDate = timesheetRecord.ReportForDate,
                            FactTrackedInMinutes = timesheetRecord.FactTrackedInMinutes,
                            BillableTrackedInMinutes = timesheetRecord.BillableTrackedInMinutes,
                            ProjectId = project.Id,
                            ProjectName = project.Name,
                            TaskName = timesheetRecord.TaskName!,
                            IsProjectActive = true
                        }
                    ]
                )
                .WithPaging(1, 100)
                .WithTotal(1), []
        );

        // Act
        var filter = $"$filter= ( reportYear eq {timesheetDateTime.Year} and reportMonthOfYear eq {timesheetDateTime.Month} and clientId eq {clientId} and projectId eq {project.Id} )";
        var response = await client.GetTimesheetByUserIdAsPostMethodAsync(userId: userId, filter: filter);

        // Assert
        response.Should().BeEquivalentTo(expectedBody);
    }

    [Theory]
    [InlineData("")]
    [InlineData("?")]
    [InlineData("?top=50")]
    [InlineData("?skip=0")]
    [InlineData("?top=0&skip=50")]
    [InlineData("?top=50&skip=-1")]
    [InlineData("?top=50&skip=15")]
    [InlineData("?top=26&skip=50")]
    [InlineData("?top=50&skip=100&select=id,TaskName")]
    [InlineData("?top=50&skip=0&apply=groupby((position), aggregate(id with count as Count))")]
    public async Task GetTimesheetsByUserIdWhenRequestIsInvalidResponseWithBadRequestResult(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = _factory.EmployeeUserId!.Value;

        // Act
        var response = await client.GetTimesheetByUserIdWithRawSearchAsHttpMessageAsync(userId: userId,
            query: query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("?top=10&skip=0")]
    [InlineData("?top=10&skip=50")]
    [InlineData("?top=50&skip=50&filter=contains(taskName,'Task')")]
    [InlineData("?top=50&skip=50&orderby=taskName")]
    [InlineData("?top=50&skip=50&filter=contains(taskName,'Task')&orderby=taskName")]
    [InlineData("?top=50&skip=50&filter=taskName eq 'Task'")]
    [InlineData("?$filter=contains(tolower(taskName), tolower(''))&$orderby=taskName asc&$top=50&$skip=0")]
    public async Task GetTimesheetsByUserIdWhenRequestIsValidResponseWithOk(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = _factory.EmployeeUserId!.Value;

        // Act
        var response = await client.GetTimesheetByUserIdWithRawSearchAsHttpMessageAsync(userId: userId,
            query: query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTimesheetsByUserIdWhenRequestContainsValidFilterResponseWithExpectedTimesheet()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var employeeUserId = _factory.EmployeeUserId!.Value;
        var date = DateTime.UtcNow;
        var week = date.GetWeekOfYear();

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: employeeUserId, projectId: projectId);

        await client.TrackFactAsync(userId: employeeUserId, projectId: projectId);

        var createdTimesheet = await client.TrackBillableAsync(userId: employeeUserId, projectId: projectId);

        var expectedItem = new UserTimesheetDto
        {
            Id = createdTimesheet.Id,
            ProjectId = project.Id,
            ProjectName = project.Name,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = createdTimesheet.TaskName!,
            BillableTrackedInMinutes = createdTimesheet.BillableTrackedInMinutes,
            FactTrackedInMinutes = createdTimesheet.FactTrackedInMinutes,
            IsProjectActive = true
        };

        var filter =
            $"( (ReportYear eq {date.Year}) and (ReportWeekOfYear eq {week}) and (ClientId eq {clientId}) and (ProjectId eq {project.Id}) )";

        // Act
        var response = await client.GetTimesheetByUserIdAsync(userId: employeeUserId, filter: filter);

        // Assert
        var items = response.PageableListOfItems.Items;
        items.Should().ContainEquivalentOf(expectedItem);
    }

    [Fact]
    public async Task GetTimesheetsByUserIdWhenManagerHasPublicProjectAndGetTimesheetsOfAnyUserNotFromManagedProjectsResponseWithExpectedUser()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var pmoHeadEmail = _factory.PmoHeadOfDepartmentEmail!;
        var pmoHeadId = _factory.PmoHeadOfDepartmentUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;

        var createdEmployee = await client.CreateEmployee();
        var userId = createdEmployee!.Id;

        await client.CreateProject(clientId: clientId, managerId: pmoHeadId, isPublic: true, isBillable: false);

        await client.AuthAs(pmoHeadEmail);

        // Act
        var response = await client.GetTimesheetByUserIdAsHttpMessageAsync(userId: userId, onlyManagedProjects: true);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTimesheetsByUserIdAuthedAsEmplyeeHavingLastMonthOfYearSubmittedResponseWithCorrectBlockedDays()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var createdEmployee = await client.CreateEmployee();
        var userId = createdEmployee!.Id;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        var currentDate = DateTime.UtcNow.AddYears(1);
        var timesheetDateTime = new DateOnly(currentDate.Year, 12, 31);

        await client.CreateProjectAssignment(userId: userId, projectId: projectId, onProjectFrom: timesheetDateTime);

        var timesheet = await client.TrackFactAsync(userId: userId, projectId: projectId, date: timesheetDateTime);

        var expectedResult = new UserTimesheetsWithBlockedDaysDto(
            PageableListOfItemsExtensions.ToPageable(
                    [
                        new UserTimesheetDto
                        {
                            Id = timesheet.Id,
                            ProjectId = project.Id,
                            ProjectName = project.Name,
                            ReportForDate = timesheetDateTime,
                            TaskName = timesheet.TaskName!,
                            FactTrackedInMinutes = timesheet.FactTrackedInMinutes,
                            BillableTrackedInMinutes = null,
                            IsProjectActive = true
                        }
                    ]
                )
                .WithPaging(1, 100)
                .WithTotal(1),
            [
                timesheetDateTime.AddDays(-1),
                timesheetDateTime
            ]);

        // Act
        await client.AuthAs(createdEmployee.Email);

        await client.UpdateTimesheetStatusAsync(userId: userId, month: timesheetDateTime.Month, year: timesheetDateTime.Year,
            status: TimesheetSubmitStatusType.UnderReview);


        var startDate = timesheetDateTime.AddDays(-1).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var endDate = timesheetDateTime.AddDays(5).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var filter = $"reportForDate ge {startDate} and reportForDate le {endDate}";

        var response = await client.GetTimesheetByUserIdAsync(userId: userId, filter: filter);

        // Assert
        response.Should().BeEquivalentTo(expectedResult);
    }
}
