using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class EmployeeDeleteTrackedTimeAsPmScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public EmployeeDeleteTrackedTimeAsPmScenario(FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task DeleteTrackedTimeAsProjectManager()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee Delete timesheet as Project manager scenario");

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
                        return await httpClient.CreateEmployee(departmentId: "PMO");
                    });

                    return createdUser;
                });
        });

        var createdProject = await scenario.Step("Create project", async () =>
        {
            var project = await scenario.Step($"[Projects] Call POST endpoint managed by created user #{createdUser!.Id}", async () =>
            {
                var body = new CreateProjectInput
                {
                    ClientId = _testFactory.ModsenClientId!.Value,
                    Color = null,
                    IsBillable = false,
                    IsPublic = false,
                    Name = $"{Guid.CreateVersion7().ToAlphabetic()}",
                    ManagerId = createdUser.Id
                };

                return await httpClient.CreateProject(body);
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

                var body = await response.Content.ReadFromJsonAsync<UserProjectAssignmentOutput>();
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

        var authedManager = await scenario.Step("Login as created manager", async () =>
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
        var taskName = Guid.CreateVersion7().ToString();

        var createdTimesheet = await scenario.Step($"Track time to project {createdProject.Name}", async () =>
        {
            var project = await scenario.Step("Get available managed projects", async () =>
            {
                var managedProjects = await httpClient.GetProjectsByUserIdAsync(
                    userId: authedManager.Id,
                    onlyManaged: true);

                return managedProjects.First(x => x.Name == createdProject.Name);
            });

            var createdTimesheet = await scenario.Step("Add time to project", async () =>
            {
                var input = new UpsertFactDto
                {
                    ProjectId = project.Id,
                    ReportForDate = reportDay,
                    TaskName = taskName,
                    FactTrackedInMinutes = 60
                };

                var timesheet = await scenario.Step("Call POST endpoint", async () =>
                {
                    return await httpClient.TrackFactAsync(input: input);
                });

                await scenario.Step("Verify response", () =>
                {
                    timesheet.Should().NotBeNull();
                    timesheet.ProjectId.Should().Be(project.Id);
                    timesheet.CreatedById.Should().Be(authedManager.Id);
                });

                return timesheet;
            });

            await scenario.Step("Check tracked time on timesheets page", async () =>
            {
                var timesheets = await scenario.Step("Call GET endpoint", async () =>
                {
                    return await httpClient.GetTimesheetByUserIdAsync(userId: authedManager.Id,
                        top: 100,
                        skip: 0,
                        filter: $"Id eq {createdTimesheet.Id}");
                });

                await scenario.Step("Verify response", () =>
                {
                    timesheets.Should().NotBeNull();
                    timesheets.PageableListOfItems.Items.Should().ContainSingle();

                    var timesheet = timesheets.PageableListOfItems.Items.First();
                    timesheet.ProjectId.Should().Be(project.Id);
                    timesheet.TaskName.Should().Be(taskName);
                    timesheet.FactTrackedInMinutes.Should().Be(60);
                });
            });

            return createdTimesheet;
        });

        await scenario.Step("Delete timesheet", async () =>
        {
            await scenario.Step("Call DELETE endpoint", async () =>
            {
                var responseFact = await httpClient.DeleteFactAsHttpMessageAsync(id: createdTimesheet.Id);

                responseFact.StatusCode.Should().Be(HttpStatusCode.OK);
            });

            await scenario.Step("Check deleted timesheet on timesheets page", async () =>
            {
                var timesheets = await scenario.Step("Call GET endpoint", async () =>
                {
                    return await httpClient.GetTimesheetByUserIdAsync(userId: authedManager.Id,
                        top: 100,
                        skip: 0,
                        filter: $"Id eq {createdTimesheet.Id}");
                });

                await scenario.Step("Verify deleted timesheet", () =>
                {
                    timesheets.Should().NotBeNull();
                    timesheets.PageableListOfItems.Items.Should().BeEmpty();
                });
            });
        });
    }
}
