using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class CeoDeactivateUserWithActiveProjectScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public CeoDeactivateUserWithActiveProjectScenario(FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task DeactivateUserWithActiveProjectScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[CEO]",
            title: "CEO remove user with active project scenario");

        await scenario.Step("Login", async () =>
        {
            await scenario.Step("Call auth endpoint", async () => await httpClient.AuthAsCeo());

            await scenario.Step(
                "Call 'Me' endpoint for getting info about current user", async () =>
                {
                    return await httpClient.GetCurrentUser();
                });
        });

        var projects = await scenario.Step("Get projects lookup", async () =>
        {
            return await httpClient.GetProjectsLookupAsync();
        });

        var user = await scenario.Step("Create user", async () =>
        {
            return await httpClient.CreateEmployee();
        });

        var project = projects.First();

        await scenario.Step("Assign the project to the user", async () =>
        {
            var response = await scenario.Step("Call POST endpoint", async () =>
            {
                return await httpClient.CreateProjectAssignmentAsHttpMessageAsync(userId: user!.Id,
                    projectId: project.Id);
            });

            await scenario.Step("Verify if user-project assignment has been created", async () =>
            {
                response.IsSuccessStatusCode.Should().BeTrue();

                var body = await response.Content.ReadFromJsonAsync<UserProjectAssignmentOutput>();
                var assignmentId = body!.Id;

                body.ProjectId.Should().Be(project.Id);
                body.UserId.Should().Be(user!.Id);

                var assignment = await scenario.Step("Get assignment by retrieved Id", async () =>
                {
                    return await httpClient.GetProjectAssignmentAsync(assignmentId);
                });

                await scenario.Step("Verify if retrieved user-project assignment valid", async () =>
                {
                    assignment.Id.Should().Be(assignmentId);
                    assignment.ProjectId.Should().Be(project.Id);
                    assignment.UserId.Should().Be(user.Id);
                });
            });
        });

        var response = await scenario.Step("Try to deactivate user", async () =>
        {
            return await httpClient.SetUserInactiveAsHttpMessage(user!.Id);

        });

        await scenario.Step("Verify if user was not deactivated", async () =>
        {
            var retrievedUser = await scenario.Step("Get user by id", async () =>
            {
                return await httpClient.GetUserAsync(user!.Id);
            });

            response.IsSuccessStatusCode.Should().BeFalse();
            retrievedUser!.IsActive.Should().BeTrue();
        });
    }
}
