using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class CeoUpdateClientScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public CeoUpdateClientScenario(
        FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task UpdateClientScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[CEO]",
            title: "CEO update Client scenario");

        await scenario.Step("Login", async () =>
        {
            await scenario.Step("Call auth endpoint", async () => await httpClient.AuthAsCeo());

            await scenario.Step("Call 'Me' endpoint for getting info about current user", async () =>
            {
                return await httpClient.GetCurrentUser();
            });
        });

        var createdClient = await scenario.Step("Create Client", async () =>
        {
            var clientName = "CreateClientTestNameForUpdate";

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

            await scenario.Step("Get clients with created client name", async () =>
            {
                var filter = $"((IsActive eq true) and (((contains(tolower(Name), tolower('{createdClient!.Name}'))))))";
                var orderBy = "Name asc";

                return await httpClient.GetClientsWithSearchAsync(filter: filter, orderBy: orderBy);
            });

            return createdClient;
        });

        await scenario.Step("Update client", async () =>
        {
            var newClientName = "NewClientTestName";
            var newClientComments = "NewClientTestComments";
            var newClientRegisteredLocation = 2;

            var clients = await scenario.Step("Get clients with created client name", async () =>
            {
                var filter = $"((IsActive eq true) and (((contains(tolower(Name), tolower('{createdClient!.Name}'))))))";
                var orderBy = "Name asc";

                return await httpClient.GetClientsWithSearchAsync(filter: filter, orderBy: orderBy);
            });

            var clientForUpdateId = clients.Items.First(client => client.Id == createdClient!.Id).Id;

            await scenario.Step("Get client info", async () =>
            {
                await httpClient.GetClientAsync(clientForUpdateId);
            });

            var updatedClient = await scenario.Step("Update client", async () =>
            {
                var updateClientInput = new UpdateClientInput
                {
                    Id = clientForUpdateId,
                    Name = newClientName,
                    Comments = newClientComments,
                    RegisteredLocation = newClientRegisteredLocation
                };

                return await httpClient.UpdateClientAsync(updateClientInput);
            });

            var clientsWithFilters = await scenario.Step("Get clients with updated client name", async () =>
            {
                var filter = $"((IsActive eq true) and (((contains(tolower(Name), tolower('{updatedClient.Name}'))))))";
                var orderBy = "Name asc";

                return await httpClient.GetClientsWithSearchAsync(filter: filter, orderBy: orderBy);
            });

            await scenario.Step("Verify if client was successfully updated", () =>
            {
                updatedClient.Should().NotBeNull();
                updatedClient.IsActive.Should().BeTrue();
                updatedClient.Name.Should().BeEquivalentTo(newClientName);
                updatedClient.Comments.Should().BeEquivalentTo(newClientComments);
                updatedClient.RegisteredLocation.Should().BeEquivalentTo("Poland");
                clientsWithFilters.Items.Should().NotBeEmpty();
                clientsWithFilters.Items.Should().Contain(client => client.Id == createdClient!.Id);
            });
        });
    }
}
