using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class EmployeeUpdateTaskNameAsPMOScenario
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public EmployeeUpdateTaskNameAsPMOScenario(FunctionalTestFactory testFactory, ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task UpdateTaskNameTimeWhenAuthAsPMOShouldResponseWithOk()
    {
        using var client = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee Update taskName when auth as employee from pmo department scenario");

        await scenario.Step("Login as CEO", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await client.AuthAsCeo();
            });

            var user = await scenario.Step(
                "Call 'Me' endpoint for getting info about current user",
                async () =>
                {
                    return await client.GetCurrentUser();
                });

            await scenario.Step("Verify info about user", () =>
            {
                user.Should().NotBeNull();
                user.DepartmentId.Should().BeNull();
                user.Email.Should().NotBeNullOrEmpty();
            });

            return user;
        });

        var createdUser = await scenario.Step("Create new employee from pmo department", async () =>
        {
            await client.AuthAsCeo();
            return await scenario.Step("Call 'Users' endpoint to create new user", async () =>
            {
                var createdUser = await scenario.Step("Create new user", async () =>
                {
                    return await client.CreateEmployee(departmentId: "PMO");
                });

                return createdUser;
            });
        });

        var createdProject = await scenario.Step("Created project", async () =>
        {
            await client.AuthAsCeo();
            var project = await scenario.Step("[Projects] Call POST endpoint", async () =>
            {
                var response = await client.CreateProject(clientId: _testFactory.ModsenClientId!.Value,
                    isBillable: false,
                    isPublic: true);

                return response;
            });

            await scenario.Step("Assign the project to the user", async () =>
            {
                var response = await scenario.Step("Call POST endpoint", async () =>
                {
                    return await client.CreateProjectAssignmentAsHttpMessageAsync(userId: createdUser!.Id,
                        projectId: project!.Id, isTimeBillable: false);
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
                        return await client.GetProjectAssignmentAsync(assignmentId);
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

            return project;
        });

        var createdTimesheet = await scenario.Step("Create timesheet record", async () =>
        {
            await client.AuthAsCeo();
            var reportForDate = new DateOnly(2024, 8, 1);

            await client.CreateProjectAssignmentAsHttpMessageAsync(userId: createdUser!.Id,
                projectId: createdProject!.Id,
                isTimeBillable: false,
                onProjectFrom: reportForDate);

            await client.AuthAs(createdUser.Email);

            var createdRecord = await scenario.Step("Call Timesheet POST endpoint", async () =>
            {
                var timesheetOutput = await client.TrackFactAsync(new UpsertFactDto
                {
                    ProjectId = createdProject.Id,
                    ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow.Date),
                    TaskName = "MMS-00001",
                    FactTrackedInMinutes = 240,
                    UserId = createdUser.Id
                });

                return timesheetOutput;
            });

            await scenario.Step("Verify response", () =>
            {
                createdRecord.Should().NotBeNull();
                createdRecord.UserId.Should().Be(createdUser.Id);
                createdRecord.ProjectId.Should().Be(createdProject.Id);
                createdRecord.TaskName.Should().Be("MMS-00001");
                createdRecord.FactTrackedInMinutes.Should().Be(240);

                return Task.CompletedTask;
            });

            return createdRecord;
        });

        await scenario.Step("Update timesheet", async () =>
        {
            await client.AuthAsCeo();

            var updatedTaskName = await scenario.Step("Update taskName", async () =>
            {
                var input = new UpdateTaskNameDto
                {
                    UserId = createdTimesheet.UserId,
                    ProjectId = createdTimesheet.ProjectId,
                    EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                    StartDate = new DateOnly(2023, 5, 2),
                    OldTaskName = createdTimesheet.TaskName!,
                    TaskName = "MMS2NewTaskName"
                };

                var response = await client.UpdateTaskNameAsHttpMessageAsync(input: input);

                response.StatusCode.Should().Be(HttpStatusCode.OK);

                return await response.Content.ReadFromJsonAsync<TimesheetDto>(
                    _jsonOptions,
                    _cancellationToken);
            });

            await scenario.Step("Verify if taskName was successfully updated", () =>
            {
                updatedTaskName.Should().NotBeNull();
                updatedTaskName.TaskName.Should().Be(updatedTaskName.TaskName);
            });
        });
    }
}
