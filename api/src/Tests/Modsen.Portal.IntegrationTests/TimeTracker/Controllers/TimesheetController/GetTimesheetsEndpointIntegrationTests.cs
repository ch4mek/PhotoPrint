using System.Net;
using System.Text.Json;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.IntegrationTests.TestFramework.Helpers.TimeTracker;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.IntegrationTests.TestFramework.IntegrationTestFactoryExtensions;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.TimesheetController;

[Collection("TimeTrackerCollection")]
public class GetTimesheetsEndpointIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetTimesheetsEndpointIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _factory.CleanTimesheets();
    }

    [Fact]
    public async Task GetTimesheetsWhenAuthedAsEmployeeShouldResponseWithForbiddenResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var reportDateRange = DateRangeHelper.GetDateRangeFromYear(2024);

        var response = await client.GetTimesheetsAsHttpMessageAsync(dateFrom: reportDateRange.DateFrom,
            dateTo: reportDateRange.DateTo);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetTimesheetsWhenHeadOfDepartmentTriesToGetTimesheetsOfOtherDepartmentShouldResponseOKWithEmptyList()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var clientId = _factory.ModsenClientId!.Value;
        var timesheetDateTime = new DateOnly(2022, 07, 17);

        var employee = await client.CreateEmployee(departmentId: "Mobile");
        var userId = employee!.Id;

        await client.AuthAsHeadOfDepartment();
        var head = await client.GetCurrentUser();

        var expectedUser = new UserTimesheetSnapshotDto
        {
            Color = head.Color,
            Timesheets = [],
            UserDepartmentId = "Net",
            TotalFactTime = 0,
            TotalBillableTime = 0,
            UserId = head.Id,
            UserName = head.LastName + " " + head.FirstName,
            IsActive = true,
            Role = head.Position
        };
        await client.AuthAsCeo();

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId, onProjectFrom: timesheetDateTime);

        await client.CreateFactTimesheetAsHttpMessageAsync(new UpsertFactDto
            {
                ProjectId = project.Id,
                ReportForDate = timesheetDateTime.AddDays(-40),
                TaskName = "TimeTracker 123",
                FactTrackedInMinutes = 60,
                UserId = employee.Id
            });

        await client.AuthAsHeadOfDepartment();

        var expectedBody = new PageableUserTimesheets(
            PageableListOfItemsExtensions.ToPageable([expectedUser])
                .WithPaging(1, 100)
                .WithTotal(1));

        var reportDateRange = DateRangeHelper.GetDateRangeFromYear(2023);

        // Act
        var response = await client.GetTimesheetsAsync(dateFrom: reportDateRange.DateFrom,
            dateTo: reportDateRange.DateTo,
            userId: expectedUser.UserId,
            departmentId: "Net");

        // Assert
        response.Should().BeEquivalentTo(expectedBody);
    }

    [Fact]
    public async Task GetTimesheetsWhenHeadOfDepartmentGetTimesheetsOfUserWithoutTrackedTimeAsProjectManagerShouldResponseOKWithUserInfo()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var clientId = _factory.ModsenClientId!.Value;
        var pmoHeadEmail = _factory.PmoHeadOfDepartmentEmail!;
        var pmoHeadId = _factory.PmoHeadOfDepartmentUserId!.Value;

        var employee = await client.CreateEmployee(departmentId: "Mobile");
        var userId = employee!.Id;

        var expectedUser = new UserTimesheetSnapshotDto
        {
            Color = employee.Color,
            Timesheets = [],
            UserDepartmentId = "Mobile",
            TotalFactTime = 0,
            TotalBillableTime = 0,
            UserId = employee.Id,
            UserName = employee.LastName + " " + employee.FirstName,
            Role = employee.Position,
            IsActive = true
        };

        var project = await client.CreateProject(clientId: clientId, managerId: pmoHeadId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        await client.AuthAs(pmoHeadEmail);

        var expectedBody = new PageableUserTimesheets(
            PageableListOfItemsExtensions.ToPageable([expectedUser])
                .WithPaging(1, 100)
                .WithTotal(1));

        // Act
        var reportDateRange = DateRangeHelper.GetDateRangeFromMonth(1000, 7);

        var response = await client.GetTimesheetsAsync(dateFrom: reportDateRange.DateFrom,
            dateTo: reportDateRange.DateTo,
            userId: userId,
            onlyManagedProjects: true);

        // Assert
        response.Should().BeEquivalentTo(expectedBody);
    }

    [Fact]
    public async Task GetTimesheetsWhenAnyInactiveUserHaveTrackedHoursShouldResponseOKWithInactiveUser()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var clientId = _factory.ModsenClientId!.Value;
        var timesheetDateTime = new DateOnly(2023, 07, 17);

        const string filter = "((departmentId eq 'SysAdmin') and (position eq 'HeadOfDepartment'))";
        var users = await client.GetUsersWithSearch(filter: filter, top: 1);
        var head = users.Items.First();
        var headId = head.Id;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: headId, projectId: projectId,
            onProjectFrom: timesheetDateTime,
            onProjectTo: DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1));

        await client.TrackFactAsync(userId: headId, projectId: projectId, date: timesheetDateTime);

        await client.SetUserInactive(headId);

        // Act
        var reportDateRange = DateRangeHelper.GetDateRangeFromYear(timesheetDateTime.Year);

        var response = await client.GetTimesheetsAsync(dateFrom: reportDateRange.DateFrom,
            dateTo: reportDateRange.DateTo,
            userId: headId);

        // Assert
        response.UsersTimesheets.Items.Should().Contain(item => item.UserId == head.Id);
    }

    [Fact]
    public async Task GetTimesheetsWhenAuthedAsHeadOfDepartmentThatCanBeProjectManagerAndUserFromAnotherDepartmentHaveTimesheetsOnProjectManagedByHeadShouldReturnOnlyTimesheetsFromManagedProject()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var clientId = _factory.ModsenClientId!.Value;
        var pmoHeadEmail = _factory.PmoHeadOfDepartmentEmail!;
        var pmoHeadId = _factory.PmoHeadOfDepartmentUserId!.Value;
        var timesheetDateTime = new DateOnly(2022, 07, 17);

        var employee = await client.CreateEmployee(departmentId: "Mobile");
        var userId = employee!.Id;

        var projectWithManager = await client.CreateProject(clientId: clientId, managerId: pmoHeadId);
        var projectWithManagerId = projectWithManager!.Id;

        var projectWithoutManager = await client.CreateProject(clientId: clientId);
        var projectWithoutManagerId = projectWithoutManager!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectWithManagerId,
            onProjectFrom: timesheetDateTime);
        await client.CreateProjectAssignment(userId: userId, projectId: projectWithoutManagerId,
            onProjectFrom: timesheetDateTime);

        var timesheetForProjectWithManager = await client.TrackBillableAsync(userId: userId, projectId: projectWithManagerId, date: timesheetDateTime);

        await client.TrackBillableAsync(userId: userId, projectId: projectWithoutManagerId, date: timesheetDateTime);

        var expectedBody = new PageableUserTimesheets(
            PageableListOfItemsExtensions.ToPageable([
                    new UserTimesheetSnapshotDto
                    {
                        UserId = employee.Id,
                        UserName = employee.LastName + " " + employee.FirstName,
                        Role = employee.Position,
                        Color = employee.Color,
                        IsActive = true,
                        UserDepartmentId = "Mobile",
                        TotalFactTime = timesheetForProjectWithManager.FactTrackedInMinutes,
                        TotalBillableTime = timesheetForProjectWithManager.BillableTrackedInMinutes,
                        Timesheets =
                        [
                            new TimesheetSnapshotDto
                            {
                                Id = timesheetForProjectWithManager.Id,
                                FactTrackedInMinutes = timesheetForProjectWithManager.FactTrackedInMinutes,
                                BillableTrackedInMinutes = timesheetForProjectWithManager.BillableTrackedInMinutes,
                                ReportForDate = timesheetForProjectWithManager.ReportForDate
                            }
                        ]
                    }
                ])
                .WithPaging(1, 100)
                .WithTotal(1));

        // Act
        await client.AuthAs(pmoHeadEmail);

        var reportDateRange = DateRangeHelper.GetDateRangeFromYear(timesheetDateTime.Year);

        var response = await client.GetTimesheetsAsync(dateFrom: reportDateRange.DateFrom,
            dateTo: reportDateRange.DateTo,
            userId: userId,
            onlyManagedProjects: true);

        // Assert
        response.Should().BeEquivalentTo(expectedBody);
    }

    [Fact]
    public async Task GetTimesheetsWhenAuthedAsHeadOfDepartmentThatCanBeProjectManagerAndUserFromAnotherDepartmentHaveTimesheetsOnProjectManagedByHeadShouldReturnOnlyFilteredTimesheetsFromManagedProject()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var clientId = _factory.ModsenClientId!.Value;
        var pmoHeadEmail = _factory.PmoHeadOfDepartmentEmail!;
        var pmoHeadId = _factory.PmoHeadOfDepartmentUserId!.Value;
        var timesheetDateTime = new DateOnly(2022, 07, 17);

        var employee = await client.CreateEmployee(departmentId: "Mobile");
        var userId = employee!.Id;

        var projectWithManager = await client.CreateProject(clientId: clientId, managerId: pmoHeadId);
        var projectWithManagerId = projectWithManager!.Id;

        var anotherProjectWithManager = await client.CreateProject(clientId: clientId, managerId: pmoHeadId);
        var anotherProjectWithManagerId = anotherProjectWithManager!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectWithManagerId,
            onProjectFrom: timesheetDateTime);

        await client.CreateProjectAssignment(userId: userId, projectId: anotherProjectWithManagerId,
            onProjectFrom: timesheetDateTime);

        var timesheetForProjectWithManager = await client.TrackBillableAsync(userId: userId, projectId: projectWithManagerId, date: timesheetDateTime);

        await client.TrackBillableAsync(userId: userId, projectId: anotherProjectWithManagerId, date: timesheetDateTime);

        var expectedBody = new PageableUserTimesheets(
            PageableListOfItemsExtensions.ToPageable([
                    new UserTimesheetSnapshotDto
                    {
                        UserId = employee.Id,
                        UserName = employee.LastName + " " + employee.FirstName,
                        Role = employee.Position,
                        Color = employee.Color,
                        IsActive = true,
                        UserDepartmentId = "Mobile",
                        TotalFactTime = timesheetForProjectWithManager.FactTrackedInMinutes,
                        TotalBillableTime = timesheetForProjectWithManager.BillableTrackedInMinutes,
                        Timesheets =
                        [
                            new TimesheetSnapshotDto
                            {
                                Id = timesheetForProjectWithManager.Id,
                                FactTrackedInMinutes = timesheetForProjectWithManager.FactTrackedInMinutes,
                                BillableTrackedInMinutes = timesheetForProjectWithManager.BillableTrackedInMinutes,
                                ReportForDate = timesheetForProjectWithManager.ReportForDate
                            }
                        ]
                    }
                ])
                .WithPaging(1, 100)
                .WithTotal(1));

        // Act
        await client.AuthAs(pmoHeadEmail);

        var reportDateRange = DateRangeHelper.GetDateRangeFromYear(timesheetDateTime.Year);

        var response = await client.GetTimesheetsAsync(dateFrom: reportDateRange.DateFrom,
            dateTo: reportDateRange.DateTo,
            projectId: projectWithManagerId,
            onlyManagedProjects: true);

        // Assert
        response.Should().BeEquivalentTo(expectedBody);
    }

    [Fact]
    public async Task GetTimesheetsWhenAuthedAsEmployeeThanCanBeProjectManagerAndTryToGetTimesheetsForManagedProjectShouldReturnOnlyTimesheetsFromManagedProject()
    {
        //Arrange

        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var pmoUser = await client.CreateEmployee(departmentId: "PMO");
        var managerId = pmoUser!.Id;

        var employee = await client.CreateEmployee(departmentId: "Java");
        var userId = employee!.Id;

        var project = await client.CreateProject(clientId: clientId, managerId: managerId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        var timesheet = await client.TrackBillableAsync(userId: userId, projectId: projectId);

        var expectedBody = new PageableUserTimesheets(
                PageableListOfItemsExtensions.ToPageable([
                    new UserTimesheetSnapshotDto
                    {
                        UserId = employee.Id,
                        UserName = employee.LastName + " " + employee.FirstName,
                        Role = employee.Position,
                        Color = employee.Color,
                        IsActive = true,
                        UserDepartmentId = employee.DepartmentId!,
                        TotalFactTime = timesheet.FactTrackedInMinutes,
                        TotalBillableTime = timesheet.BillableTrackedInMinutes,
                        Timesheets =
                        [
                            new TimesheetSnapshotDto
                            {
                                Id = timesheet.Id,
                                FactTrackedInMinutes = timesheet.FactTrackedInMinutes,
                                BillableTrackedInMinutes = timesheet.BillableTrackedInMinutes,
                                ReportForDate = timesheet.ReportForDate
                            }
                        ]
                    }
                ])
                .WithPaging(1, 100)
                .WithTotal(1));

        //Act
        await client.AuthAs(pmoUser.Email);

        var reportDateRange = DateRangeHelper.GetDateRangeFromYear(timesheet.ReportForDate.Year);

        var response = await client.GetTimesheetsAsync(dateFrom: reportDateRange.DateFrom,
            dateTo: reportDateRange.DateTo,
            userId: userId,
            onlyManagedProjects: true);

        //Assert
        response.Should().BeEquivalentTo(expectedBody);

    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task
        GetTimesheetsWhenCurrentRoleCannotBeManagerAndTryToGetTimesheetsForManagedProjectShouldReturnForbidden(string role)
    {
        //Arrange
        var client = _factory.CreateClient();

        //Act
        await client.AuthAsRole(role);
        var reportDateRange = DateRangeHelper.GetDateRangeFromYear(2023);

        var response = await client.GetTimesheetsAsHttpMessageAsync(dateFrom: reportDateRange.DateFrom,
            dateTo: reportDateRange.DateTo,
            onlyManagedProjects: true);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData("")]
    [InlineData("?")]
    [InlineData("?skip=0")]
    [InlineData("?top=0&skip=50")]
    [InlineData("?top=50&skip=-1")]
    [InlineData("?top=50&skip=15")]
    [InlineData("?top=50&skip=100&select=id,TaskName")]
    [InlineData("?top=50&skip=50&filter=UserDepartment eq 1")]
    [InlineData("?top=50&skip=0&apply=groupby((position), aggregate(id with count as Count))")]
    public async Task GetTimesheetsWhenRequestIsInvalidResponseWithBadRequestResult(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetTimesheetsWithRawSearchAsHttpMessageAsync(query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("?top=10&skip=0&reportForDateFrom=2023-01-01&reportForDateTo=2023-12-31")]
    [InlineData("?top=10&skip=50&reportForDateFrom=2012-01-01&reportForDateTo=2012-12-31")]
    [InlineData("?top=50&skip=50&reportForDateFrom=2012-01-01&reportForDateTo=2012-12-31&departmentId='Net'")]
    [InlineData("?top=50&skip=50&reportForDateFrom=2023-01-01&reportForDateTo=2023-12-31&projectId=1")]
    [InlineData("?top=50&skip=50&reportForDateFrom=2023-01-01&reportForDateTo=2023-12-31&clientId=1")]
    [InlineData("?top=50&skip=50&reportForDateFrom=2023-12-01&reportForDateTo=2023-12-31")]
    [InlineData("?top=50&skip=50&reportForDateFrom=2024-12-30&reportForDateTo=2025-01-05")]
    public async Task GetTimesheetsWhenRequestIsValidResponseWithOk(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetTimesheetsWithRawSearchAsHttpMessageAsync(query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTimesheetsWhenRequestContainsValidFilterResponseWithTimesheets()
    {
        // Arrange
        var client = _factory.CreateClient();

        var clientId = _factory.ModsenClientId!.Value;
        var timesheetDateTime = new DateOnly(2022, 07, 17);
        await client.AuthAsCeo();

        var employee = await client.CreateEmployee();
        var userId = employee!.Id;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId, onProjectFrom: timesheetDateTime);

        await client.TrackFactAsync(userId: userId, projectId: projectId, date: timesheetDateTime);
        await client.TrackFactAsync(userId: userId, projectId: projectId, date: timesheetDateTime.AddDays(40));

        await client.AuthAsHeadOfDepartment();

        // Act
        var reportDateRange = DateRangeHelper.GetDateRangeFromYear(timesheetDateTime.Year);

        var response = await client.GetTimesheetsAsync(dateFrom: reportDateRange.DateFrom,
            dateTo: reportDateRange.DateTo,
            userId: userId,
            departmentId: "Net");

        // Assert
        response.Should().NotBeNull();
        response.UsersTimesheets.Total.Should().BeGreaterThanOrEqualTo(1);
        response.UsersTimesheets.Items.First().Timesheets.Count.Should().BeGreaterThanOrEqualTo(2);
        response.UsersTimesheets.Items.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetTimesheetsAuthedAsGroupManagerResponseWithTimesheets()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var clientId = _factory.ModsenClientId!.Value;
        var timesheetDateTime = new DateOnly(2022, 07, 17);
        const string departmentId = "Net";

        var createdUserFromGroup = await client.CreateEmployee(resourceGroupId: 1);
        var userId = createdUserFromGroup!.Id;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId, onProjectFrom: timesheetDateTime);

        await client.TrackFactAsync(userId: userId, projectId: projectId, date: timesheetDateTime.AddDays(1));
        await client.TrackFactAsync(userId: userId, projectId: projectId, date: timesheetDateTime);

        // Act
        await client.AuthAsGroupManager();

        var reportDateRange = DateRangeHelper.GetDateRangeFromYear(timesheetDateTime.Year);

        var response = await client.GetTimesheetsAsync(dateFrom: reportDateRange.DateFrom,
                dateTo: reportDateRange.DateTo,
                userId: userId,
                departmentId: departmentId);

        // Assert
        response.Should().NotBeNull();
        response.UsersTimesheets.Total.Should().Be(1);
        response.UsersTimesheets.Items.First().Timesheets.Count.Should().Be(2);
        response.UsersTimesheets.Items.Should().NotBeNullOrEmpty();
        //
        // await CleanupProject(project.Id);
        //
        // await client.AuthAsCeo();
        //
        // var userStatus = new UserStatusInput { IsActive = false };
        //
        // await client.PutAsJsonAsync($"api/Users/{createdUserFromGroup.Id}/IsActive",
        //     userStatus, _cancellationToken);
    }

    [Fact]
    public async Task GetTimesheetsByUserIdWhenManagerHasPublicProjectAndDoesNotSpecifyClientAndProjectResponseWithExpectedUser()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var clientId = _factory.ModsenClientId!.Value;
        var pmoHeadEmail = _factory.PmoHeadOfDepartmentEmail!;
        var pmoHeadId = _factory.PmoHeadOfDepartmentUserId!.Value;

        var createdEmployee = await client.CreateEmployee();
        var userId = createdEmployee!.Id;

        await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true,
            managerId: pmoHeadId);

        var expectedBody = new PageableUserTimesheets(
            PageableListOfItemsExtensions.ToPageable([
                    new UserTimesheetSnapshotDto
                    {
                        UserId = createdEmployee.Id,
                        UserName = createdEmployee.LastName + " " + createdEmployee.FirstName,
                        Color = createdEmployee.Color,
                        IsActive = true,
                        UserDepartmentId = createdEmployee.DepartmentId!,
                        Role = createdEmployee.Position,
                        TotalFactTime = 0,
                        TotalBillableTime = 0,
                        Timesheets = []
                    }
                ])
                .WithPaging(1, 100)
                .WithTotal(1));

        await client.AuthAs(pmoHeadEmail);

        // Act
        var reportDateRange = DateRangeHelper.GetDateRangeFromYear(2022);

        var response = await client.GetTimesheetsAsync(dateFrom: reportDateRange.DateFrom,
            dateTo: reportDateRange.DateTo,
            userId: userId,
            onlyManagedProjects: true);

        // Assert
        response.Should().BeEquivalentTo(expectedBody);
    }
}
