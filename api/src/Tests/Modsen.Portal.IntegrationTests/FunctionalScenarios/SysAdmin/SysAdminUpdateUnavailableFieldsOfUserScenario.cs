using System.Net;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios.SysAdmin;

[Collection("FuncScenarioCollection")]
public class SysAdminUpdateUnavailableFieldsOfUserScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public SysAdminUpdateUnavailableFieldsOfUserScenario(FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task UpdateUnavailableFieldsOfUserScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(
            outputAdapter,
            scenarioPrefix: "[System Administrator]",
            title: "System Administrator System Administrator updates unavailable fields for user scenario");

        await scenario.Step("Login", async () =>
        {
            await scenario.Step("Call auth endpoint", async () => await httpClient.AuthAsAdmin());

            return await scenario.Step("Call 'Me' endpoint for getting info about current user", async () =>
            {
                return await httpClient.GetCurrentUser();
            });
        });

        var users = await scenario.Step("Get users lookup", async () =>
        {
            return await httpClient.GetUsersLookup(isActive: true, roles: [PortalRoleNames.Employee]);
        });

        var userId = users.Last().Id;

        var user = await scenario.Step("Get user by Id", async () =>
        {
            return await httpClient.GetUserAsync(userId);
        });

        var userToUpdate = new UpdateUserInput
        {
            Id = userId,
            Position = PortalRoleNames.HeadOfDepartment,
            Location = LocationType.Warsaw,
            DepartmentId = "Js",
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var response = await scenario.Step("Try to update unavailable fields of user", async () =>
        {
            return await httpClient.UpdateUserAsHttpMessageAsync(userToUpdate, _jsonOptions);
        });

        await scenario.Step("Verify if user haven't been updated", async () =>
        {
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            var userAfterUpdate = await scenario.Step("Get user by Id", async () =>
            {
                return await httpClient.GetUserAsync(userId);
            });

            userAfterUpdate.Should().BeEquivalentTo(user);
        });
    }
}
