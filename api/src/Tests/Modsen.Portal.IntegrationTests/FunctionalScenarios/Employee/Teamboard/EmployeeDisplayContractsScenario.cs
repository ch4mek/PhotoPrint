using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios.Teamboard;

[Collection("FuncScenarioCollection")]
public class EmployeeDisplayContractsScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public EmployeeDisplayContractsScenario(FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task DisplayContractsScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee Display user contracts scenario");

        await scenario.Step("Login as CEO", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAsCeo();
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
                user.DepartmentId.Should().BeNull();
                user.Email.Should().NotBeNullOrEmpty();
            });

            return user;
        });

        var createdUser = await scenario.Step("Create new employee", async () =>
        {
            return await scenario.Step("Create new user", async () =>
            {
                var userToCreate = new CreateUserInput
                {
                    FirstName = Guid.CreateVersion7().ToAlphabetic(),
                    LastName = Guid.CreateVersion7().ToAlphabetic(),
                    FirstNameRu = "Имя",
                    LastNameRu = "Фамилия",
                    PatronymicRu = "Отчество",
                    Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
                    Position = PortalRoleNames.Employee,
                    Location = LocationType.Minsk,
                    DepartmentId = "Net",
                    Color = "#ffffff",
                    CountryId = 1,
                    RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
                };

                return await httpClient.CreateUser(userToCreate, _jsonOptions);
            });
        });

        var contractFrom = DateOnly.FromDateTime(DateTime.Now).AddYears(-6);
        var contractTo = DateOnly.FromDateTime(DateTime.Now).AddYears(-5);

        await scenario.Step("Create new contract", async () =>
        {
            var input = new CreateUserContractInput(
                createdUser!.Id,
                contractFrom,
                contractTo,
                null, null, null, null);

            var response = await httpClient.CreateContracts(input)
                .WithResponseType<UserContractOutput>();

            return response;
        });

        await scenario.Step("Display user contracts", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAs(createdUser!.Email);
            });

            var contracts = await scenario.Step("Get user contracts", async () =>
            {
                return await httpClient.GetContracts($"top=100&skip=0&filter=userId eq {createdUser!.Id}&$orderby=contractFrom asc")
                    .WithResponseType<PageableListOfItems<OdpUserContract>>();
            });

            await scenario.Step("Verify user contracts", () =>
            {
                contracts.Items.Should().HaveCount(2);
                contracts.Items.First().ContractFrom.Should().Be(contractFrom);
                contracts.Items.First().ContractTo.Should().Be(contractTo);
            });
        });
    }
}
