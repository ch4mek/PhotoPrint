using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class CeoCreateUserScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public CeoCreateUserScenario(
        FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task CreateUserScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[CEO]",
            title: "CEO create User scenario");

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

        await scenario.Step("Create User", async () =>
        {
            var userFirstName = "CreateUserTestName";

            await scenario.Step("Get all users", async () =>
            {
                var orderBy = "id desc";

                return await httpClient.GetUsersWithSearch(orderBy: orderBy);
            });

            var projects = await scenario.Step("Get projects lookup",
                async () =>
                {
                    return await httpClient.GetProjectsLookupAsync();
                });

            var firstProjectId = projects[0].Id;

            var createdUser = await scenario.Step("Call User POST endpoint", async () =>
            {
                return await httpClient.CreateEmployee(firstName: userFirstName);
            });

            await scenario.Step("Create user-project assignment", async () =>
            {
                await httpClient.CreateProjectAssignment(userId: createdUser!.Id, projectId: firstProjectId);
            });

            await scenario.Step("Get all users", async () =>
            {
                var orderBy = "id desc";
                return await httpClient.GetUsersWithSearch(orderBy: orderBy);
            });

            var users = await scenario.Step("Get users with created user first name", async () =>
            {
                var filter = $"contains(firstName,'{createdUser!.FirstName}')";
                var orderBy = "id desc";
                return await httpClient.GetUsersWithSearch(orderBy: orderBy, filter: filter);
            });

            await scenario.Step("Verify if user was successfully created", () =>
            {
                createdUser.Should().NotBeNull();
                createdUser.FirstName.Should().BeEquivalentTo(userFirstName);
                users.Items.Should().NotBeEmpty();
                users.Items.Should().Contain(user => user.Id == createdUser.Id);
            });
        });
    }

}
