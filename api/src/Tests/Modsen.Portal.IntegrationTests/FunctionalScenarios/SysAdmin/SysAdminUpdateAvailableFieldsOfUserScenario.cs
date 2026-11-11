using System.Net;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios.SysAdmin;

[Collection("FuncScenarioCollection")]
public class SysAdminUpdateAvailableFieldsOfUserScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public SysAdminUpdateAvailableFieldsOfUserScenario(FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task UpdateAvailableFieldsOfUserScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(
            outputAdapter,
            scenarioPrefix: "[System Administrator]",
            title: "System Administrator updates available fields for user scenario");

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

        var userId = users.First().Id;

        var user = await scenario.Step("Get user by Id", async () =>
        {
            return await httpClient.GetUserAsync(userId);
        });

        var firstName = Guid.CreateVersion7().ToAlphabetic();
        var lastName = Guid.CreateVersion7().ToAlphabetic();
        var email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com";

        var userToUpdate = new UpdateUserInput
        {
            Id = userId,
            Position = user!.Position!,
            DepartmentId = user.DepartmentId,
            Location = user.Location,
            FirstName = firstName,
            LastName = lastName,
            FirstNameRu = "тест",
            LastNameRu = "тест",
            Email = email,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var response = await scenario.Step("Update user", async () =>
        {
            return await httpClient.UpdateUserAsHttpMessageAsync(userToUpdate, _jsonOptions);
        });

        await scenario.Step("Verify if user have been updated", async () =>
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var userAfterUpdate = await scenario.Step("Get user by Id", async () =>
            {
                return await httpClient.GetUserAsync(userId);
            });

            userAfterUpdate!.FirstName.Should().Be(firstName);
            userAfterUpdate.Email.Should().Be(email);
            userAfterUpdate.LastName.Should().Be(lastName);
        });
    }
}
