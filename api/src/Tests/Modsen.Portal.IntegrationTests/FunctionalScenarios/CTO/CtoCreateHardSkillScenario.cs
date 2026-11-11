using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Output;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using NScenario;
using Xunit.Abstractions;

namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class CtoCreateHardSkillScenario
{
    private readonly FunctionalTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly JsonSerializerOptions _serializerOptions;

    public CtoCreateHardSkillScenario(
        FunctionalTestFactory factory,
        ITestOutputHelper outputHelper)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _serializerOptions = _factory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task CreateHardSkillScenario()
    {
        using var httpClient = _factory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(
            outputAdapter,
            scenarioPrefix: "[CTO]",
            title: "CTO Create hard skill scenario");

        var hardSkillName = Guid.CreateVersion7().ToAlphabetic();
        var hardSkillGroupId = "CID";

        await scenario.Step("Login", async () =>
        {
            await scenario.Step(
                "Call auth endpoint",
                () => httpClient.AuthAsCto());

            var user = await scenario.Step("Call 'Me' endpoint for getting info about current user", async () =>
            {
                return await httpClient.GetFromJsonAsync<UserOutput>(
                    "api/users/current",
                    _serializerOptions,
                    _cancellationToken);
            });

            await scenario.Step("Verify info about user", () =>
            {
                user.Should().NotBeNull();
                user.DepartmentId.Should().BeNull();
                user.Email.Should().NotBeNullOrEmpty();
                user.Position.Should().Be(PortalRoleNames.Cto);
            });
        });

        var createdHardSkill = await scenario.Step("Create hard skill", async () =>
        {
            var response = await scenario.Step("Create hard skill", async () =>
            {
                return await httpClient.PostAsJsonAsync(
                    "api/hard-skills",
                    new CreateHardSkillInput
                    (
                        Name: hardSkillName,
                        HardSkillGroupId: hardSkillGroupId
                    ),
                    _cancellationToken);
            });

            await scenario.Step(
                "Validate response status code",
                () => response.StatusCode.Should().Be(HttpStatusCode.Created));

            return await response.Content.ReadFromJsonAsync<UserContractOutput>(
                _serializerOptions,
                _cancellationToken);
        });

        await scenario.Step("Check created hard skill", async () =>
        {
            var hardSkill = await scenario.Step("Get hard skill by id", async () =>
            {
                return await httpClient.GetFromJsonAsync<HardSkillByIdOutput>(
                    $"api/hard-skills/{createdHardSkill!.Id}",
                    _cancellationToken);
            });

            await scenario.Step("Validate response body", () =>
            {
                hardSkill.Should().NotBeNull();
                hardSkill.Name.Should().Be(hardSkillName);
                hardSkill.HardSkillGroupId.Should().Be(hardSkillGroupId);
            });
        });
    }
}
