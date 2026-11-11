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
public class CtoCreateSpecializationTemplateScenario
{
    private readonly FunctionalTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly JsonSerializerOptions _serializerOptions;

    public CtoCreateSpecializationTemplateScenario(
        FunctionalTestFactory factory,
        ITestOutputHelper outputHelper)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _serializerOptions = _factory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task CreateSpecializationTemplateScenario()
    {
        using var httpClient = _factory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(
            outputAdapter,
            scenarioPrefix: "[CTO]",
            title: "CTO Create specialization template scenario");

        var name = Guid.CreateVersion7().ToAlphabetic();
        List<string>? departmentIds = ["Net"];
        List<int>? hardSkillIds = [1];

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

        var createdSpecializationTemplate = await scenario.Step("Create specialization template", async () =>
        {
            var response = await scenario.Step("Create specialization template", async () =>
            {
                return await httpClient.PostAsJsonAsync(
                    "api/specialization-templates",
                    new CreateSpecializationTemplateInput
                    (
                        Name: name,
                        DepartmentIds: departmentIds,
                        HardSkillids: hardSkillIds
                    ),
                    _cancellationToken);
            });

            await scenario.Step(
                "Validate response status code",
                () => response.StatusCode.Should().Be(HttpStatusCode.OK));

            return await response.Content.ReadFromJsonAsync<SpecializationTemplateOutput>(
                _serializerOptions,
                _cancellationToken);
        });

        await scenario.Step("Check created specialization template", async () =>
        {
            var specializationTemplate = await scenario.Step("Get specialization template by id", async () =>
            {
                return await httpClient.GetFromJsonAsync<SpecializationTemplateWithDepartmentsAndHardSkillsOutput>(
                    $"api/specialization-templates/{createdSpecializationTemplate!.Id}",
                    _cancellationToken);
            });

            await scenario.Step("Validate response body", () =>
            {
                specializationTemplate.Should().NotBeNull();
                specializationTemplate.Name.Should().Be(name);
                specializationTemplate.DepartmentDtos.Select(x => x.DepartmentId)
                .FirstOrDefault().Should().Be(departmentIds.FirstOrDefault());
                specializationTemplate.HardSkills.Select(x => x.Id)
                .FirstOrDefault().Should().Be(hardSkillIds.FirstOrDefault());
            });
        });
    }
}
