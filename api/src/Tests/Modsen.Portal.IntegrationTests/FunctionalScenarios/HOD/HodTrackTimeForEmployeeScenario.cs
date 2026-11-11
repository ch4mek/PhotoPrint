using System.Net;
using System.Net.Http.Json;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;
using NScenario;
using Xunit.Abstractions;

namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

public class HodTrackTimeForEmployeeScenario : IClassFixture<FunctionalTestFactory>
{
    private readonly FunctionalTestFactory _factory;
    private readonly ITestOutputHelper _outputHelper;

    public HodTrackTimeForEmployeeScenario(
        FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _factory = testFactory;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task TrackTimeForEmployeeScenario()
    {
        using var httpClient = _factory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(
            outputAdapter,
            scenarioPrefix: "[Head of Department]",
            title: "Head of Department Track time for Employee scenario");

        var taskName = Guid.CreateVersion7().ToAlphabetic();
        const int trackedInMinutes = 240;
        var reportForDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-1));

        var currentUser = await scenario.Step("Login", async () =>
        {
            await scenario.Step(
                "Call auth endpoint",
                () => httpClient.AuthAsHeadOfDepartment());

            var user = await scenario.Step("Call 'Me' endpoint for getting info about current user", async () =>
            {
                return await httpClient.GetCurrentUser();
            });

            await scenario.Step("Verify info about user", () =>
            {
                user.Should().NotBeNull();
                user.DepartmentId.Should().NotBeNull();
                user.Email.Should().NotBeNullOrEmpty();
                user.Position.Should().Be(PortalRoleNames.HeadOfDepartment);
            });

            return user;
        });

        var timesheet = await scenario.Step("Track time for Employee", async () =>
        {
            var employee = await scenario.Step("Get an employee", async () =>
            {
                var employee = await scenario.Step("Create user", async () =>
                {
                    return await httpClient.CreateEmployee(departmentId: currentUser.DepartmentId!);
                });

                var employeeFullName = $"{employee!.FirstName} {employee.LastName}";

                var fullEmployeeInfo = await scenario.Step($"[{employeeFullName}] Get full user info", async () =>
                {
                    return await httpClient.GetUserAsync(employee.Id);
                });

                await scenario.Step($"[{employeeFullName}] Verify user", () =>
                {
                    fullEmployeeInfo.Should().NotBeNull();
                    fullEmployeeInfo.Id.Should().Be(employee.Id);
                    fullEmployeeInfo.FirstName.Should().Be(employee.FirstName);
                    fullEmployeeInfo.LastName.Should().Be(employee.LastName);
                    fullEmployeeInfo.Color.Should().Be(employee.Color);
                    fullEmployeeInfo.AvatarFileId.Should().Be(employee.AvatarFileId);
                    fullEmployeeInfo.DepartmentId.Should().Be(currentUser.DepartmentId);
                });

                return fullEmployeeInfo!;
            });

            var employeeFullName = $"{employee.FirstName} {employee.LastName}";

            var project = await scenario.Step($"[{employeeFullName}] Get project", async () =>
            {
                var privateProjects = await scenario.Step($"[{employeeFullName}] Get private project", async () =>
                {
                    return await httpClient.GetProjectsLookupAsync(isActive: true, isPublic: false);
                });

                var existingUserProjects = await scenario.Step("Get user-project assignments", async () =>
                {
                    return await httpClient.GetUserProjectsLookupAsync(userId: employee.Id);
                });

                var firstPrivateProject = await scenario.Step("Pick first private project not assigned to user", () =>
                {
                    return privateProjects.First(x => existingUserProjects.All(y => y.ProjectId != x.Id));
                });

                await scenario.Step("Assign the project to the user", async () =>
                {
                    var response = await scenario.Step("Call POST endpoint", async () =>
                    {
                        return await httpClient.CreateProjectAssignmentAsHttpMessageAsync(userId: employee.Id,
                            projectId: firstPrivateProject.Id,
                            onProjectFrom: reportForDate);
                    });

                    await scenario.Step("Verify if user-project assignment has been created", async () =>
                    {
                        response.StatusCode.Should().Be(HttpStatusCode.OK);

                        var body = await response.Content.ReadFromJsonAsync<UserProjectAssignmentOutput>();
                        var assignmentId = body!.Id;

                        body.ProjectId.Should().Be(firstPrivateProject.Id);
                        body.UserId.Should().Be(employee.Id);

                        var assignment = await scenario.Step("Get assignment by retrieved Id", async () =>
                        {
                            return await httpClient.GetProjectAssignmentAsync(assignmentId);
                        });

                        await scenario.Step("Verify if retrieved user-project assignment valid", async () =>
                        {
                            assignment.Id.Should().Be(assignmentId);
                            assignment.ProjectId.Should().Be(firstPrivateProject.Id);
                            assignment.UserId.Should().Be(employee.Id);
                        });
                    });
                });

                var fullProjectInfo = await scenario.Step($"[{firstPrivateProject.Name}] Get full project info", async () =>
                {
                    return await httpClient.GetProjectByIdAsync(firstPrivateProject.Id);
                });

                await scenario.Step($"[{firstPrivateProject}] Verify project", () =>
                {
                    fullProjectInfo.Should().NotBeNull();
                    fullProjectInfo.Id.Should().Be(firstPrivateProject.Id);
                    fullProjectInfo.Name.Should().Be(firstPrivateProject.Name);
                    fullProjectInfo.Color.Should().Be(firstPrivateProject.Color);
                    fullProjectInfo.IsPublic.Should().BeFalse();
                    fullProjectInfo.IsBillable.Should().BeTrue();
                });

                return fullProjectInfo;
            });

            return await scenario.Step($"[{project.Name}] Track time", async () =>
            {
                var createdTimesheet = await scenario.Step($"[{project.Name}] Create timesheet record", async () =>
                {
                    return await httpClient.TrackFactAsync(
                        new UpsertFactDto
                        {
                            ProjectId = project.Id,
                            ReportForDate = reportForDate,
                            TaskName = taskName,
                            FactTrackedInMinutes = trackedInMinutes,
                            UserId = employee.Id
                        });
                });

                await scenario.Step("Verify response", () =>
                {
                    createdTimesheet.Should().NotBeNull();
                    createdTimesheet.ProjectId.Should().Be(project.Id);
                    createdTimesheet.CreatedById.Should().Be(currentUser.Id);
                    createdTimesheet.UserId.Should().Be(employee.Id);
                });

                return createdTimesheet;
            });
        });

        await scenario.Step("Check created timesheet on timesheets page", async () =>
        {
            var employeeTimesheets = await scenario.Step("Get timesheets by user and project", async () =>
            {
                return await httpClient.GetTimesheetByUserIdAsync(userId: timesheet.UserId,
                    filter: $"ProjectId eq {timesheet.ProjectId}");
            });

            var timesheetToCheck = await scenario.Step("Pick created timesheet", () =>
            {
                return employeeTimesheets.PageableListOfItems.Items.First(x => x.TaskName == timesheet.TaskName);
            });

            await scenario.Step("Verify timesheet response", () =>
            {
                timesheetToCheck.Should().NotBeNull();
                timesheetToCheck.Id.Should().NotBe(0);
                timesheetToCheck.ProjectId.Should().Be(timesheet.ProjectId);
                timesheetToCheck.ReportForDate.Should().Be(reportForDate);
                timesheetToCheck.TaskName.Should().Be(taskName);
                timesheetToCheck.FactTrackedInMinutes.Should().Be(trackedInMinutes);
            });
        });

        await scenario.Step("Check dashboard for tracked task", async () =>
        {
            var dashboardForUser = await scenario.Step("Get dashboard report", async () =>
            {
                return await httpClient.GetDashboardUserProjectAsync(
                    userId: timesheet.UserId,
                    projectId: timesheet.ProjectId);
            });

            var dashboardRecord = await scenario.Step("Pick activity by task name", () =>
            {
                return dashboardForUser
                    .Find(x => x.TaskName == timesheet.TaskName);
            });

            await scenario.Step("Verify dashboard report response", () =>
            {
                dashboardRecord.Should().NotBeNull();
                dashboardRecord.TaskName.Should().Be(taskName);
                dashboardRecord.TrackedInMinutes.Should().Be(trackedInMinutes);
                dashboardRecord.ReportForDate.Should().Be(reportForDate);
            });
        });
    }
}
