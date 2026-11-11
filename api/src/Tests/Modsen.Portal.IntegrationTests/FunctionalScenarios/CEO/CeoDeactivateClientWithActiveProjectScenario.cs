using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class CeoDeactivateClientWithActiveProjectScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public CeoDeactivateClientWithActiveProjectScenario(
        FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task DeactivateClientWithActiveProjectScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[CEO]",
            title: "CEO remove client with active project scenario");

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

        var createdClient = await scenario.Step("Create Client", async () =>
        {
            var clientName = "CreateClientName";

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

            await scenario.Step("Verify if client was successfully created", () =>
            {
                createdClient.Should().NotBeNull();
                createdClient.Name.Should().BeEquivalentTo(clientName);
            });

            return createdClient;
        });

        var createdProject = await scenario.Step("Create project", async () =>
        {
            var clients = await scenario.Step("Get clients lookup", async () =>
            {
                return await httpClient.GetClientsLookupAsync();
            });

            await scenario.Step("Get users lookup", async () =>
            {
                return await httpClient.GetUsersLookup();
            });

            var projectClient = clients!.First(client => client.Id == createdClient!.Id);

            var createProjectModel = new CreateProjectInput
            {
                ClientId = projectClient.Id,
                IsBillable = true,
                IsPublic = false,
                Name = "RemoveClientWithActiveProjectScenario",
                Color = "#8A8E3B"
            };

            var createdProject = await scenario.Step("Create new project", async () =>
            {
                return await httpClient.CreateProject(createProjectModel);
            });

            await scenario.Step("Check if project was created", async () =>
            {
                var project = await httpClient.GetProjectByIdAsync(createdProject!.Id);

                project.Id.Should().Be(createdProject.Id);
                project.ClientId.Should().Be(createdProject.ClientId);
                project.Name.Should().Be(createdProject.Name);
                project.Color.Should().Be(createdProject.Color);
                project.IsActive.Should().BeTrue();
            });

            return createdProject;
        });

        await scenario.Step("Try to delete client with active project", async () =>
        {
            await scenario.Step("Search clients", async () =>
            {
                var filter = "((IsActive eq true) and (((contains(tolower(Name), tolower(''))))))";
                var orderBy = "Name asc";
                await httpClient.GetClientsWithSearchAsync(filter: filter, orderBy: orderBy);
            });

            var clientToDelete = await scenario.Step("Get client", async () =>
            {
                return await httpClient.GetClientAsync(createdClient!.Id);
            });

            await scenario.Step("Verify that client exist", () =>
            {
                clientToDelete.Should().NotBeNull();
                clientToDelete.Id.Should().Be(createdClient!.Id);
            });

            var deleteClientOutput = await scenario.Step("Delete client", async () =>
            {
                return await httpClient.SetClientInactiveAsync(clientToDelete!.Id);
            });

            await scenario.Step("Search clients", async () =>
            {
                var filter = "((IsActive eq true) and (((contains(tolower(Name), tolower(''))))))";
                var orderBy = "Name asc";
                await httpClient.GetClientsWithSearchAsync(filter: filter, orderBy: orderBy);
            });

            await scenario.Step("Verify that client was not deleted", async () =>
            {
                var client = await httpClient.GetClientAsync(createdClient!.Id);

                deleteClientOutput.IsSuccessful.Should().BeFalse();
                client!.IsActive.Should().BeTrue();
            });
        });

        await scenario.Step("Deactivate project", async () =>
        {
            await scenario.Step("Deactivate project", async () =>
            {
                await httpClient.SetProjectInactiveAsync(createdProject!.Id);
            });

            await scenario.Step("Check if project was deactivated", async () =>
            {
                var project = await httpClient.GetProjectByIdAsync(createdProject!.Id);

                project.IsActive.Should().BeFalse();
            });
        });

        await scenario.Step("Delete client without active projects", async () =>
        {
            await scenario.Step("Search clients", async () =>
            {
                var filter = "((IsActive eq true) and (((contains(tolower(Name), tolower(''))))))";
                var orderBy = "Name asc";
                await httpClient.GetClientsWithSearchAsync(filter: filter, orderBy: orderBy);
            });

            var clientToDelete = await scenario.Step("Get client", async () =>
            {
                return await httpClient.GetClientAsync(createdClient!.Id);
            });

            await scenario.Step("Verify that client exist", () =>
            {
                clientToDelete.Should().NotBeNull();
                clientToDelete.Id.Should().Be(createdClient!.Id);
            });

            var deleteClientOutput = await scenario.Step("Delete client", async () =>
            {
                return await httpClient.SetClientInactiveAsync(clientToDelete!.Id);
            });

            await scenario.Step("Search clients", async () =>
            {
                var filter = "((IsActive eq true) and (((contains(tolower(Name), tolower(''))))))";
                var orderBy = "Name asc";
                await httpClient.GetClientsWithSearchAsync(filter: filter, orderBy: orderBy);
            });

            await scenario.Step("Verify that client was deleted", async () =>
            {
                var response = await httpClient.GetClientAsHttpMessageAsync(createdClient!.Id);

                deleteClientOutput.IsSuccessful.Should().BeTrue();
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            });
        });
    }
}
