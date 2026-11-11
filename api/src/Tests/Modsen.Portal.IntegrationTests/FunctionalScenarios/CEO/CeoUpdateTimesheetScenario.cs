using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("UnstableFuncScenarioCollection")]
public class CeoUpdateTimesheetScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public CeoUpdateTimesheetScenario(FunctionalTestFactory testFactory, ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task UpdateTimesheetScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[CEO]",
            title: "CEO change the existing time entry for the user scenario");

        await scenario.Step("Login", async () =>
        {
            await scenario.Step("Call auth endpoint", async () => await httpClient.AuthAsCeo());

            await scenario.Step(
                "Call 'Me' endpoint for getting info about current user",
                async () =>
                {
                    return await httpClient.GetCurrentUser();
                });
        });

        var projects = await scenario.Step("Get projects lookup", async () =>
        {
            return await httpClient.GetProjectsLookupAsync();
        });

        var projectId = projects.First().Id;

        var users = await scenario.Step("Get all users", async () =>
        {
            var orderBy = "id desc";

            return await httpClient.GetUsersWithSearch(orderBy: orderBy);
        });

        var userId = users.Items.First(x => x.Id == 5).Id;

        var createdTimesheet = await scenario.Step("Create timesheet record", async () =>
        {
            var taskName = "task";
            var factTrackedInMinutes = 245;
            var billableTrackedInMinutes = 123;
            var reportForDate = new DateOnly(2024, 5, 12);

            await httpClient.CreateProjectAssignment(userId: userId, projectId: projectId,
                onProjectFrom: reportForDate);

            var createdTimesheet = await scenario.Step("Call Timesheet POST endpoint", async () =>
            {
                var createdTimesheetInputFact = new UpsertFactDto
                {
                    TaskName = taskName,
                    FactTrackedInMinutes = factTrackedInMinutes,
                    ReportForDate = reportForDate,
                    ProjectId = projectId,
                    UserId = userId
                };

                await httpClient.TrackFactAsync(createdTimesheetInputFact);

                var createdTimesheetInputBillable = new UpsertBillableDto
                {
                    TaskName = taskName,
                    BillableTrackedInMinutes = billableTrackedInMinutes,
                    ReportForDate = reportForDate,
                    ProjectId = projectId,
                    UserId = userId
                };

                return await httpClient.TrackBillableAsync(createdTimesheetInputBillable);
            });

            return createdTimesheet;
        });

        await scenario.Step("Update timesheet", async () =>
        {
            var taskName = "task";
            var factTrackedInMinutes = 120;
            var billableTrackedInMinutes = 245;
            var reportForDate = new DateOnly(2024, 5, 12);

            var updatedTimesheet = await scenario.Step("Update timesheet", async () =>
            {
                var createdTimesheetInputFact = new UpsertFactDto
                {
                    TaskName = taskName,
                    FactTrackedInMinutes = factTrackedInMinutes,
                    ReportForDate = reportForDate,
                    ProjectId = projectId,
                    UserId = userId
                };

                await httpClient.TrackFactAsync(createdTimesheetInputFact);

                var createdTimesheetInputBillable = new UpsertBillableDto
                {
                    TaskName = taskName,
                    BillableTrackedInMinutes = billableTrackedInMinutes,
                    ReportForDate = reportForDate,
                    ProjectId = projectId,
                    UserId = userId
                };

                return await httpClient.TrackBillableAsync(createdTimesheetInputBillable);
            });

            var timesheetsWithFilters = await scenario.Step("Get timesheet with updated tracked time", async () =>
            {
                return await httpClient.GetTimesheetByUserIdAsync(createdTimesheet.UserId, top: 20);
            });

            await scenario.Step("Verify if time was successfully updated", () =>
            {
                // AssertionOptions.FormattingOptions.MaxLines = 500;

                updatedTimesheet.Should().NotBeNull();
                updatedTimesheet.BillableTrackedInMinutes.Should().Be(billableTrackedInMinutes);
                updatedTimesheet.FactTrackedInMinutes.Should().Be(factTrackedInMinutes);
                timesheetsWithFilters.PageableListOfItems.Items.Should().Contain(timesheet => timesheet.Id == createdTimesheet.Id);
            });
        });
    }
}
