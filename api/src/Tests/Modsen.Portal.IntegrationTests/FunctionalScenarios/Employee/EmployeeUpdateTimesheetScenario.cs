using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;
using NScenario;
using Xunit.Abstractions;

namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class EmployeeUpdateTimesheetScenario
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public EmployeeUpdateTimesheetScenario(FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _serializerOptions = _testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task UpdateTimesheetScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);
        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee Update timesheet scenario");

        var reportDay = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(2));

        var currentUser = await scenario.Step("Login", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAsEmployee();
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

        var isCurrentUserAbleToSeeBillableTrackedTime = false;

        var createdTimesheet = await scenario.Step("Track time to first available projects", async () =>
        {
            var project = await scenario.Step("Get available project", async () =>
            {
                return (await httpClient.GetProjectsByUserIdAsync(userId: currentUser.Id)).First();
            });

            isCurrentUserAbleToSeeBillableTrackedTime = project.IsCurrentUserAbleToSeeBillableTrackedTime;

            var createdTimesheet = await scenario.Step("Add time to project", async () =>
            {
                var input = new UpsertFactDto
                {
                    ProjectId = project.Id,
                    ReportForDate = reportDay.AddDays(1),
                    TaskName = $"Onboarding to {project.Name}",
                    FactTrackedInMinutes = 60
                };

                var timesheet = await scenario.Step($"[{project.Name}] Call POST endpoint", async () =>
                {
                    var response = await httpClient.TrackFactAsHttpMessageAsync(input);

                    return await response.Content.ReadFromJsonAsync<TimesheetCoreDto>(
                        _serializerOptions, _cancellationToken);
                });

                await scenario.Step("Verify response", () =>
                {
                    timesheet.Should().NotBeNull();
                    timesheet.ProjectId.Should().Be(project.Id);
                    timesheet.CreatedById.Should().Be(currentUser.Id);
                });

                return timesheet;
            });

            await scenario.Step("Check tracked time on timesheets page", async () =>
            {
                var timesheets = await scenario.Step("Call GET endpoint", async () =>
                {
                    return await httpClient.GetTimesheetByUserIdAsync(userId: currentUser.Id,
                        filter: $"Id eq {createdTimesheet!.Id}");
                });

                await scenario.Step($"[{project.Name}] Verify response", () =>
                {
                    timesheets.Should().NotBeNull();
                    timesheets.PageableListOfItems.Items.Should().ContainSingle();
                    var timesheet = timesheets.PageableListOfItems.Items.First();
                    timesheet.ProjectId.Should().Be(project.Id);
                    timesheet.TaskName.Should().Be($"Onboarding to {project.Name}");
                    timesheet.FactTrackedInMinutes.Should().Be(60);
                });
            });

            return createdTimesheet;
        });

        await scenario.Step("Update timesheet", async () =>
        {
            var updateFactTrackedTimeDto = new UpsertFactDto
            {
                ProjectId = createdTimesheet!.ProjectId,
                ReportForDate = createdTimesheet.ReportForDate,
                TaskName = createdTimesheet.TaskName!,
                FactTrackedInMinutes = 120,
                UserId = createdTimesheet.UserId,
            };

            await scenario.Step("Call Patch endpoint to update timesheet trackedTime", async () =>
            {
                var response = await httpClient.TrackFactAsHttpMessageAsync(updateFactTrackedTimeDto);

                response.StatusCode.Should().Be(HttpStatusCode.OK);
            });
            if(isCurrentUserAbleToSeeBillableTrackedTime)
            {
                var updateBillableTrackedTimeDto = new UpsertBillableDto
                {
                    ProjectId = createdTimesheet.ProjectId,
                    ReportForDate = createdTimesheet.ReportForDate,
                    TaskName = createdTimesheet.TaskName!,
                    BillableTrackedInMinutes = 120,
                    UserId = createdTimesheet.UserId,
                };

                await scenario.Step("Call Patch endpoint to update timesheet trackedTime", async () =>
                {
                    var response = await httpClient.TrackBillableAsHttpMessageAsync(updateBillableTrackedTimeDto);

                    response.StatusCode.Should().Be(HttpStatusCode.OK);
                });
            }

            var updateTaskTimesheetDto = new UpdateTaskNameDto
            {
                UserId = createdTimesheet.UserId,
                ProjectId = createdTimesheet.ProjectId,
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(4)),
                StartDate = new DateOnly(2022, 5, 2),
                OldTaskName = createdTimesheet.TaskName!,
                TaskName = Guid.CreateVersion7().ToString()
            };

            await scenario.Step("Call Patch endpoint to update timesheet taskName", async () =>
            {
                var response = await httpClient.UpdateTaskNameAsHttpMessageAsync(updateTaskTimesheetDto);

                response.StatusCode.Should().Be(HttpStatusCode.OK);
            });

            await scenario.Step("Check updated timesheet on timesheets page", async () =>
            {
                var timesheets = await scenario.Step("Call Get endpoint", async () =>
                {
                    return await httpClient.GetTimesheetByUserIdAsync(userId: currentUser.Id,
                        filter: $"Id eq {createdTimesheet.Id}");
                });

                await scenario.Step("Verify updated timesheet", () =>
                {
                    timesheets.Should().NotBeNull();
                    timesheets.PageableListOfItems.Items.Should().ContainSingle();
                    var timesheet = timesheets.PageableListOfItems.Items.First();
                    timesheet.ProjectId.Should().Be(timesheet.ProjectId);
                    timesheet.TaskName.Should().Be(updateTaskTimesheetDto.TaskName);
                    timesheet.FactTrackedInMinutes.Should().Be(updateFactTrackedTimeDto.FactTrackedInMinutes);
                });
            });
        });
    }
}
