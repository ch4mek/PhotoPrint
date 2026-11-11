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
public class EmployeeTrackTimeForCurrentWeekScenario
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public EmployeeTrackTimeForCurrentWeekScenario(FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _serializerOptions = _testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task TrackTimeForCurrentWeekScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee Track time for current week scenario");

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
            await scenario.Step("Call endpoint to create a user", async () => await httpClient.AuthAsCeo());

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

        await scenario.Step("Create project", async () =>
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

            await scenario.Step($"Assign the project {project.Name} to the user [UserId: {createdUser!.Id}]", async () =>
            {
                var response = await scenario.Step("Call POST endpoint", async () =>
                {
                    return await httpClient.CreateProjectAssignmentAsHttpMessageAsync(userId: createdUser.Id,
                        projectId: project.Id);
                });

                await scenario.Step("Verify if user-project assignment has been created", async () =>
                {
                    response.StatusCode.Should().Be(HttpStatusCode.OK);

                    var body = await response.Content.ReadFromJsonAsync<UserProjectAssignmentOutput>(_cancellationToken);
                    var assignmentId = body!.Id;

                    body.ProjectId.Should().Be(project.Id);
                    body.UserId.Should().Be(createdUser.Id);

                    var assignment = await scenario.Step("Get assignment by retrieved Id", async () =>
                    {
                        return await httpClient.GetProjectAssignmentAsync(assignmentId);
                    });

                    await scenario.Step("Verify if retrieved user-project assignment valid", () =>
                    {
                        assignment.Id.Should().Be(assignmentId);
                        assignment.ProjectId.Should().Be(project.Id);
                        assignment.UserId.Should().Be(createdUser.Id);

                        return Task.CompletedTask;
                    });
                });
            });
        });

        var project = await scenario.Step("Get project", async () =>
        {
            var projects = await scenario.Step("Get projects lookup", async () =>
            {
                return await httpClient.GetProjectsLookupAsync(roles: PortalRoleNames.Employee, isActive: true);
            });

            var projectId = projects.First().Id;

            var project = await scenario.Step("Get project by id", async () =>
            {
                return await httpClient.GetProjectByIdAsync(projectId);
            });

            await scenario.Step($"Verify if project {project.Name} has not already been assigned to the user {createdUser!.Id}", async () =>
            {
                var projectUsers = await httpClient.GetProjectUserWithFilterAsync(
                    filter: $"ProjectId eq {projectId} and UserId eq {createdUser.Id}");

                projectUsers.Items.Should().BeEmpty();
            });

            return project;
        });


        await scenario.Step($"Assign the project {project.Name} to the user [UserId: {createdUser!.Id}]", async () =>
        {
            var response = await scenario.Step("Call POST endpoint", async () =>
            {
                return await httpClient.CreateProjectAssignmentAsHttpMessageAsync(userId: createdUser.Id,
                    projectId: project.Id);
            });

            await scenario.Step("Verify if user-project assignment has been created", async () =>
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var body = await response.Content.ReadFromJsonAsync<UserProjectAssignmentOutput>(_cancellationToken);
                var assignmentId = body!.Id;

                body.ProjectId.Should().Be(project.Id);
                body.UserId.Should().Be(createdUser.Id);

                var assignment = await scenario.Step("Get assignment by retrieved Id", async () =>
                {
                    return await httpClient.GetProjectAssignmentAsync(assignmentId);
                });

                await scenario.Step("Verify if retrieved user-project assignment valid", () =>
                {
                    assignment.Id.Should().Be(assignmentId);
                    assignment.ProjectId.Should().Be(project.Id);
                    assignment.UserId.Should().Be(createdUser.Id);

                    return Task.CompletedTask;
                });
            });
        });

        var currentUser = await scenario.Step("Login", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAs(createdUser.Email);
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
        var reportDay1 = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var reportDay2 = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(15));

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
                    var input1 = new UpsertFactDto
                    {
                        ProjectId = projectLookupElement.Id,
                        ReportForDate = reportDay,
                        TaskName = $"Onboarding to {projectLookupElement.Name} ({Guid.CreateVersion7():N})",
                        FactTrackedInMinutes = 60
                    };

                    var input2 = new UpsertFactDto
                    {
                        ProjectId = projectLookupElement.Id,
                        ReportForDate = reportDay1,
                        TaskName = $"Onboarding to {projectLookupElement.Name} ({Guid.CreateVersion7():N})",
                        FactTrackedInMinutes = 60
                    };

                    var input3 = new UpsertFactDto
                    {
                        ProjectId = projectLookupElement.Id,
                        ReportForDate = reportDay2,
                        TaskName = $"Onboarding to {projectLookupElement.Name} ({Guid.CreateVersion7():N})",
                        FactTrackedInMinutes = 60
                    };

                    var timesheet = await scenario.Step($"[{projectLookupElement.Name}] Call POST endpoint", async () =>
                    {
                        var response = await httpClient.TrackFactAsHttpMessageAsync(input1);

                        if (!response.IsSuccessStatusCode)
                        {
                            var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>(_cancellationToken);
                            exception?.ErrorInfo.Should().BeNullOrEmpty();
                        }

                        return await response.Content.ReadFromJsonAsync<TimesheetCoreDto>(
                            _serializerOptions, _cancellationToken);
                    });

                    await scenario.Step($"[{projectLookupElement.Name}] Verify response", () =>
                    {
                        timesheet.Should().NotBeNull();
                        timesheet.ProjectId.Should().Be(projectLookupElement.Id);
                        timesheet.CreatedById.Should().Be(currentUser.Id);
                    });

                    timesheets.Add(timesheet!);

                    var timesheet1 = await scenario.Step($"[{projectLookupElement.Name}] Call POST endpoint", async () =>
                    {
                        var response = await httpClient.TrackFactAsHttpMessageAsync(input2);

                        if (!response.IsSuccessStatusCode)
                        {
                            var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>(_cancellationToken);
                            exception?.ErrorInfo.Should().BeNullOrEmpty();
                        }

                        return await response.Content.ReadFromJsonAsync<TimesheetCoreDto>(
                            _serializerOptions, _cancellationToken);
                    });

                    await scenario.Step($"[{projectLookupElement.Name}] Verify response", () =>
                    {
                        timesheet1.Should().NotBeNull();
                        timesheet1.ProjectId.Should().Be(projectLookupElement.Id);
                        timesheet1.CreatedById.Should().Be(currentUser.Id);
                    });

                    timesheets.Add(timesheet1!);

                    var timesheet2 = await scenario.Step($"[{projectLookupElement.Name}] Call POST endpoint", async () =>
                    {
                        var response = await httpClient.TrackFactAsHttpMessageAsync(input3);

                        if (!response.IsSuccessStatusCode)
                        {
                            var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>(_cancellationToken);
                            exception?.ErrorInfo.Should().BeNullOrEmpty();
                        }

                        return await response.Content.ReadFromJsonAsync<TimesheetCoreDto>(
                            _serializerOptions, _cancellationToken);
                    });

                    await scenario.Step($"[{projectLookupElement.Name}] Verify response", () =>
                    {
                        timesheet2.Should().NotBeNull();
                        timesheet2.ProjectId.Should().Be(projectLookupElement.Id);
                        timesheet2.CreatedById.Should().Be(currentUser.Id);
                    });

                    timesheets.Add(timesheet2!);
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
            var diff = (7 + (reportDay.DayOfWeek - DayOfWeek.Monday)) % 7;

            var startOfWeek = reportDay.AddDays(-1 * diff);
            var endOfWeek = startOfWeek.AddDays(6);

            var dashboard = await scenario.Step("Call dashboard report endpoint", async () =>
            {
                return await httpClient.GetDashboardAsync(top: 200);
            });

            var trackedInMinutesWeek = timesheets
                .Where(x => x.ReportForDate >= startOfWeek && x.ReportForDate <= endOfWeek)
                .Where(x => x.ReportForDate.Year == reportDay.Year)
                .Select(x => x.FactTrackedInMinutes)
                .Sum();

            dashboard.TrackedInMinutesWeek.Should().Be(trackedInMinutesWeek);
        });
    }
}
