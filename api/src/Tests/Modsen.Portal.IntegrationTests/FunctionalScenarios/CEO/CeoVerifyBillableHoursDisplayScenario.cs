using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core.TimeTrack;
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
public class CeoVerifyBillableHoursDisplayScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CeoVerifyBillableHoursDisplayScenario(FunctionalTestFactory testFactory, ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task VerifyBillableHoursDisplayScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(
            outputAdapter,
            scenarioPrefix: "[CEO]",
            title: "CEO Verify billable hours display scenario");

        await scenario.Step("Login", async () =>
        {
            await scenario.Step("Call auth endpoint", async () => await httpClient.AuthAsCeo());

            return await scenario.Step(
                "Call 'Me' endpoint for getting info about current user",
                async () =>
                {
                    return await httpClient.GetCurrentUser();
                });
        });

        var users = await scenario.Step("Get users lookup", async () =>
        {
            return await httpClient.GetUsersLookup(isActive: true, roles: ["Employee"]);
        });

        var userId = users.First().Id;

        var clients = await scenario.Step("Get clients lookup", async () =>
        {
            return await httpClient.GetClientsLookupAsync(isActive: true);
        });

        var projectToCreate = new CreateProjectInput
        {
            Name = Guid.CreateVersion7().ToAlphabetic(),
            Color = "#ffffff",
            ClientId = clients!.First().Id,
            ManagerId = null,
            IsBillable = true,
            IsPublic = false
        };

        var createResponse = await scenario.Step("Create project with assigned observed user", async () =>
        {
            return await httpClient.CreateProjectAsHttpMessageAsync(projectToCreate);
        });

        var createdProject = await createResponse.Content.ReadFromJsonAsync<ProjectOutput>(
            _jsonOptions, _cancellationToken);
        var projectId = createdProject!.Id;

        var date = DateTime.UtcNow;
        var year = date.Year;
        var month = date.Month;

        await httpClient.CreateProjectAssignment(userId: userId, projectId: projectId,
            onProjectFrom: new DateOnly(year, month, 1));

        await scenario.Step("Get timesheet of user", async () =>
        {
            return await httpClient.GetTimesheetByUserIdAsync(userId, year: year, month: month);
        });

        var reportsBefore = await scenario.Step("Get report about user before tracking new time", async () =>
        {
            return await httpClient.GetTrackedTimePerProjectReportAsync(userId: userId,
                month: month,
                year: year,
                trackedTimeType: TrackedTimeType.Billable);
        });

        var billableMinutes = 60;
        var taskName = Guid.CreateVersion7().ToAlphabetic();

        await scenario.Step("Create new timesheet", async () =>
        {
            var timesheetToCreate = new UpsertBillableDto
            {
                ProjectId = projectId,
                UserId = userId,
                TaskName = taskName,
                BillableTrackedInMinutes = billableMinutes,
                ReportForDate = new DateOnly(year, month, day: 1)
            };

            var response = await httpClient.TrackBillableAsHttpMessageAsync(timesheetToCreate);

            await scenario.Step("Verify if creation of new timesheet is successful", () =>
            {
                response.IsSuccessStatusCode.Should().BeTrue();
            });

            return await response.Content.ReadFromJsonAsync<TimesheetCoreDto>(_cancellationToken);
        });

        var reportsAfter = await scenario.Step("Get report about user after tracking new time", async () =>
        {
            return await httpClient.GetTrackedTimePerProjectReportAsync(userId: userId,
                month: month,
                year: year,
                trackedTimeType: TrackedTimeType.Billable);
        });

        await scenario.Step("Verify if billable hours have been changed", () =>
        {
            var projectDifference = reportsAfter.ExceptBy(reportsBefore.ConvertAll(r => r.ProjectId), r => r.ProjectId);
            projectDifference.Count().Should().Be(1);

            var newReport = projectDifference.First();
            newReport.ProjectId.Should().Be(projectId);
            newReport.TotalTimeInMinutes.Should().Be(billableMinutes);
        });
    }
}
