using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using NScenario;
using Xunit.Abstractions;

namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class CeoCreateClientScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly ITestOutputHelper _outputHelper;
    private readonly JsonSerializerOptions _jsonOptions;

    public CeoCreateClientScenario(
        FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _outputHelper = outputHelper;
        _jsonOptions = testFactory.JsonOptions;
    }

    [Fact]
    public async Task CreateClientScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[CEO]",
            title: "CEO create Client scenario");

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

        await scenario.Step("Create Client", async () =>
        {
            var clientName = "CreateClientTestName";

            await scenario.Step("Get all clients", async () =>
            {
                var filter = "((IsActive eq true) and (((contains(tolower(Name), tolower(''))))))";
                var orderBy = "Name asc";

                return await httpClient.GetClientsWithSearchAsync(filter: filter, orderBy: orderBy);
            });

            var createdClient = await scenario.Step("Call Client POST endpoint", async () =>
            {
                return await httpClient.CreateClientAsync(name: clientName, registeredLocationId: 1);
            });

            await scenario.Step("Get all clients", async () =>
            {
                var filter = "((IsActive eq true) and (((contains(tolower(Name), tolower(''))))))";
                var orderBy = "Name asc";

                return await httpClient.GetClientsWithSearchAsync(filter: filter, orderBy: orderBy);
            });

            var clients = await scenario.Step("Get clients with created client name", async () =>
            {
                var filter = $"((IsActive eq true) and (((contains(tolower(Name), tolower('{createdClient!.Name}'))))))";
                var orderBy = "Name asc";

                return await httpClient.GetClientsWithSearchAsync(filter: filter, orderBy: orderBy);
            });

            await scenario.Step("Verify if client was successfully created", () =>
            {
                createdClient.Should().NotBeNull();
                createdClient.Name.Should().BeEquivalentTo(clientName);
                clients.Items.Should().NotBeEmpty();
                clients.Items.Should().Contain(client => client.Id == createdClient.Id);
            });
        });
    }

}
