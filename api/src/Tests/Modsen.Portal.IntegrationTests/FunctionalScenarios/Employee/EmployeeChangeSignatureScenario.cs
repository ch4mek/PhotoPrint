using System.Text.Json;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class EmployeeChangeSignatureScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public EmployeeChangeSignatureScenario(FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task ChangeSignatureScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee changes his signature scenario");

        await scenario.Step("Login", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAsEmployee();
            });
        });

        await scenario.Step("Update signature", async () =>
        {
            var signature = new UpdateUserSignatureInput
            {
                Position = "CEO",
                Phone = "+375295001284",
                FullName = "CEO CEO"
            };

            await scenario.Step("Call signature PUT endpoint", async () =>
            {
                await httpClient.UpdateUserSignatureAsync(signature);
            });

            var updatedSignature = await scenario.Step("Get current user signature", async () =>
            {
                return await httpClient.GetCurrentUserSignature();
            });

            await scenario.Step("Verify signature information", () =>
            {
                updatedSignature.Should().BeEquivalentTo(signature);
            });
        });
    }

}
