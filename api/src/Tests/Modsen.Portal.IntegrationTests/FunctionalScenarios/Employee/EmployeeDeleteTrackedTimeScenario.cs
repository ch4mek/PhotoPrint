using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class EmployeeDeleteTrackedTimeScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public EmployeeDeleteTrackedTimeScenario(FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task DeleteTrackedTimeScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee Delete timesheet scenario");

        var reportDay = DateOnly.FromDateTime(DateTime.UtcNow);
        var taskName = Guid.CreateVersion7().ToString();

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

        var createdTimesheet = await scenario.Step("Track time to first available projects", async () =>
        {
            var project = await scenario.Step("Get available project", async () =>
            {
                return (await httpClient.GetProjectsByUserIdAsync(userId: currentUser.Id)).First();
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
                    timesheet.CreatedById.Should().Be(currentUser.Id);
                });

                return timesheet;
            });

            await scenario.Step("Check tracked time on timesheets page", async () =>
            {
                var timesheets = await scenario.Step("Call GET endpoint", async () =>
                {
                    return await httpClient.GetTimesheetByUserIdAsync(userId: currentUser.Id,
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
                var response = await httpClient.DeleteFactAsHttpMessageAsync(createdTimesheet.Id);

                response.StatusCode.Should().Be(HttpStatusCode.OK);
            });

            await scenario.Step("Check deleted timesheet on timesheets page", async () =>
            {
                var timesheets = await scenario.Step("Call GET endpoint", async () =>
                {
                    return await httpClient.GetTimesheetByUserIdAsync(userId: currentUser.Id,
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
