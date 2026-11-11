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
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios.Teamboard;


[Collection("FuncScenarioCollection")]
public class EmployeeDisplayPlanScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public EmployeeDisplayPlanScenario(FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task DisplayEquipmentScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee Employee Display user plan scenario scenario");

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
                    DepartmentId = "PMO",
                    Color = "#ffffff",
                    CountryId = 1,
                    RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
                };

                return await httpClient.CreateUser(userToCreate);
            });
        });

        const string summary = "summary";

        await scenario.Step("Create user plan", async () =>
        {
            var input = new CreateDevelopmentPlanInput
            {
                StartDate = new DateOnly(2025, 1, 1),
                DeadlineDate = new DateOnly(2025, 2, 2),
                Summary = summary,
                Notes = null,
                UserId = createdUser!.Id,
                ConversationId = null
            };

            return await httpClient.CreateDevelopmentPlans(input);
        });

        await scenario.Step("Display user plan", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAs(createdUser!.Email);
            });

            var plans = await scenario.Step("Get user plans", async () =>
            {
                return await httpClient
                    .GetDevelopmentPlans($"?top=20&skip=0&filter=userId eq {createdUser!.Id}")
                    .WithResponseType<PageableListOfItems<OdpDevelopmentPlan>>();
            });

            await scenario.Step("Verify user contracts", () =>
            {
                plans.Items.First().Summary.Should().Be(summary);
            });
        });
    }
}
