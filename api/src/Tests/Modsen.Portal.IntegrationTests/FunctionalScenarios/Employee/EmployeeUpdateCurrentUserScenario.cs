using System.Net;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class EmployeeUpdateCurrentUserScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public EmployeeUpdateCurrentUserScenario(FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task UpdateCurrentUserScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee update his profile scenario");

        await scenario.Step("Login", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAsEmployee();
            });

            await scenario.Step(
                "Call 'Me' endpoint for getting info about current user",
                async () =>
                {
                    return await httpClient.GetCurrentUser();
                });
        });

        await scenario.Step("Update profile", async () =>
        {
            var updateCurrentUserInput = new UpdateCurrentUserInput
            {
                TimeZone = "BY",
                Location = LocationType.Tbilisi,
                AvatarColor = "#4348CA",
                DepartmentId = "Net"
            };

            var updateUserResponse = await scenario.Step("Call user PUT endpoint", async () =>
            {
                return await httpClient.UpdateCurrentUserAsHttpMessageAsync(updateCurrentUserInput);
            });

            var user = await scenario.Step(
                "Get current user",
                async () =>
                {
                    return await httpClient.GetCurrentUser();
                });

            await scenario.Step("Verify user info", () =>
            {
                updateUserResponse.StatusCode.Should().Be(HttpStatusCode.OK);
                user.TimeZone.Should().Be(updateCurrentUserInput.TimeZone);
                user.Location.Should().Be(updateCurrentUserInput.Location);
            });
        });
    }
}
