using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;
using NScenario;
using Xunit.Abstractions;

namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class EmployeeTrackTimeForCurrentDayScenario
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public EmployeeTrackTimeForCurrentDayScenario(FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _serializerOptions = _testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task TrackTimeForCurrentDayScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee Track time for current day scenario");

        await scenario.Step("Login as CEO", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAsCeo();
            });

            var user = await scenario.Step(
                "Call 'Me' endpoint for getting info about current user",
                async () =>
                {
                    return await httpClient.GetCurrentUser();
                });

            await scenario.Step("Verify info about user", () =>
            {
                user.Should().NotBeNull();
                user.DepartmentId.Should().BeNull();
                user.Email.Should().NotBeNullOrEmpty();
            });

            return user;
        });

        var createdUser = await scenario.Step("Create new employee", async () =>
        {
            return await scenario.Step(
                "Call 'Users' endpoint to create new user", async () =>
                {
                    var createdUser = await scenario.Step("Create new user", async () =>
                    {
                        return await httpClient.CreateEmployee();
                    });

                    return createdUser;
                });
        });

        var createdProject = await scenario.Step("Create project", async () =>
        {
            var project = await scenario.Step("[Projects] Call POST endpoint", async () =>
            {
                var body = new CreateProjectInput
                {
                    ClientId = _testFactory.ModsenClientId!.Value,
                    Color = null,
                    IsBillable = false,
                    IsPublic = false,
                    Name = $"{Guid.CreateVersion7().ToAlphabetic()}"
                };

                var response = await httpClient.CreateProjectAsHttpMessageAsync(body);
                if (!response.IsSuccessStatusCode)
                {
                    var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>(_cancellationToken);
                    exception?.ErrorInfo.Should().BeNullOrEmpty();
                }

                return await response.Content.ReadFromJsonAsync<ProjectOutput>(
                    _serializerOptions, _cancellationToken);
            });

            await scenario.Step($"[{project!.Name}] Verify response", () =>
            {
                project.Should().NotBeNull();
            });

            return project;
        });

        await scenario.Step("Assign the project to the user", async () =>
        {
            var response = await scenario.Step("Call POST endpoint", async () =>
            {
                return await httpClient.CreateProjectAssignmentAsHttpMessageAsync(userId: createdUser!.Id,
                    projectId: createdProject.Id);
            });

            await scenario.Step("Verify if user-project assignment has been created", async () =>
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var body = await response.Content.ReadFromJsonAsync<UserProjectAssignmentOutput>(_cancellationToken);
                var assignmentId = body!.Id;

                body.ProjectId.Should().Be(createdProject.Id);
                body.UserId.Should().Be(createdUser!.Id);

                var assignment = await scenario.Step("Get assignment by retrieved Id", async () =>
                {
                    return await httpClient.GetProjectAssignmentAsync(assignmentId);
                });

                await scenario.Step("Verify if retrieved user-project assignment valid", () =>
                {
                    assignment.Id.Should().Be(assignmentId);
                    assignment.ProjectId.Should().Be(createdProject.Id);
                    assignment.UserId.Should().Be(createdUser.Id);

                    return Task.CompletedTask;
                });
            });
        });

        var project = await scenario.Step("Get project", async () =>
        {
            var projectName = Guid.CreateVersion7().ToAlphabetic();
            await scenario.Step("Create new project", async () =>
            {
                await httpClient.CreateProject(new CreateProjectInput
                {
                    Name = projectName,
                    ClientId = _testFactory.ModsenClientId!.Value,
                    IsBillable = true,
                    IsPublic = false
                });
            });

            var projects = await scenario.Step("Get projects lookup", async () =>
            {
                return await httpClient.GetProjectsLookupAsync(isActive: true, roles: PortalRoleNames.Employee);
            });

            var project = await scenario.Step($"Find created project {projectName}", () =>
            {
                return projects.FirstOrDefault(x => x.Name == projectName);
            });

            return project;
        });

        await scenario.Step("Assign the project to the user", async () =>
        {
            var response = await scenario.Step("Call POST endpoint", async () =>
            {
                return await httpClient.CreateProjectAssignmentAsHttpMessageAsync(userId: createdUser!.Id, projectId: project.Id);
            });

            await scenario.Step("Verify if user-project assignment has been created", async () =>
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var body = await response.Content.ReadFromJsonAsync<UserProjectAssignmentOutput>(_cancellationToken);
                var assignmentId = body!.Id;

                body.ProjectId.Should().Be(project!.Id);
                body.UserId.Should().Be(createdUser!.Id);

                var assignment = await scenario.Step("Get assignment by retrieved Id", async () =>
                {
                    return await httpClient.GetProjectAssignmentAsync(assignmentId);
                });

                await scenario.Step("Verify if retrieved user-project assignment valid", () =>
                {
                    assignment.Id.Should().Be(assignmentId);
                    assignment.ProjectId.Should().Be(project.Id);
                    assignment.UserId.Should().Be(createdUser.Id);
                });
            });
        });

        var currentUser = await scenario.Step("Login", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAs(createdUser!.Email);
            });

            var user = await scenario.Step(
                "Call 'Me' endpoint for getting info about current user",
                async () =>
                {
                    return await httpClient.GetCurrentUser();
                });

            await scenario.Step("Verify info about user", () =>
            {
                user.Should().NotBeNull();
                user.DepartmentId.Should().NotBeNull();
                user.Email.Should().NotBeNullOrEmpty();
            });

            return user;
        });

        var reportDay = DateOnly.FromDateTime(DateTime.UtcNow);

        var timesheets = await scenario.Step("Track time to available projects", async () =>
        {
            var createdTimesheets = await scenario.Step("Add time to projects", async () =>
            {
                var timesheets = new List<TimesheetCoreDto>();

                var projects = await scenario.Step("Get available projects", async () =>
                {
                    return await httpClient.GetProjectsByUserIdAsync(currentUser.Id);
                });

                foreach (var projectLookupElement in projects)
                {
                    var input = new UpsertFactDto
                    {
                        ProjectId = projectLookupElement.Id,
                        ReportForDate = reportDay,
                        TaskName = $"Onboarding to {projectLookupElement.Name} {Guid.CreateVersion7():N}",
                        FactTrackedInMinutes = 60
                    };

                    var timesheet = await scenario.Step($"[{projectLookupElement.Name}] Call POST endpoint", async () =>
                    {
                        return await httpClient.TrackFactAsync(input);
                    });

                    await scenario.Step($"[{projectLookupElement.Name}] Verify response", () =>
                    {
                        timesheet.Should().NotBeNull();
                        timesheet.ProjectId.Should().Be(projectLookupElement.Id);
                        timesheet.CreatedById.Should().Be(currentUser.Id);
                    });

                    timesheets.Add(timesheet);
                }

                return timesheets;
            });

            await scenario.Step("Check tracked time on timesheets page", async () =>
            {
                var timesheets = await scenario.Step("Call GET endpoint", async () =>
                {
                    return await httpClient.GetTimesheetByUserIdAsync(userId: currentUser.Id, top: 200);
                });

                await scenario.Step("Verify response", () =>
                {
                    timesheets.Should().NotBeNull();

                    var items = timesheets.PageableListOfItems.Items;

                    foreach (var createdTimesheet in createdTimesheets)
                    {
                        var timesheet = items.Single(x => x.Id == createdTimesheet.Id);

                        timesheet.ProjectId.Should().Be(createdTimesheet.ProjectId);
                        timesheet.TaskName.Should().Be(createdTimesheet.TaskName);
                        timesheet.FactTrackedInMinutes.Should().Be(createdTimesheet.FactTrackedInMinutes);
                        timesheet.ReportForDate.Should().Be(createdTimesheet.ReportForDate);
                    }
                });
            });

            return createdTimesheets;
        });

        await scenario.Step("Check tracked time on dashboard", async () =>
        {
            var dashboard = await scenario.Step("Call dashboard report endpoint", async () =>
            {
                return await httpClient.GetDashboardAsync(top: timesheets.Count,
                    filter: $"reportForDate eq {reportDay.ToString("yyyy-MM-dd")}");
            });

            await scenario.Step("Verify dashboard report response", () =>
            {
                dashboard.Should().NotBeNull();
                var trackedInMinutesDay = timesheets
                    .Where(x => x.ReportForDate == reportDay)
                    .Select(x => x.FactTrackedInMinutes)
                    .Sum();

                dashboard.TrackedInMinutesDay.Should().Be(trackedInMinutesDay);
            });
        });
    }
}
