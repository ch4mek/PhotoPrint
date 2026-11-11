using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Identity.Controllers.Contracts.Output;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests.RequestDto;
using NScenario;
using Xunit.Abstractions;

namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class CtoGetUserSkillsSnapshotScenario
{
    private readonly FunctionalTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly JsonSerializerOptions _serializerOptions;

    public CtoGetUserSkillsSnapshotScenario(
        FunctionalTestFactory factory,
        ITestOutputHelper outputHelper)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _serializerOptions = _factory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task GetUserSkillsSnapshotScenario()
    {
        using var httpClient = _factory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(
            outputAdapter,
            scenarioPrefix: "[CTO]",
            title: "CTO Get user hard skills snapshot scenario");

        List<string>? departmentIds = ["Net"];
        List<int>? hardSkillIds = [1];
        var snapshotDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var seniorityLevelId = "SeniorL2";
        var hardSkillSeniorityLevelId = "Senior";
        var hardSkillSeniorityLevelName = "Senior";

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

        var createdUser = await scenario.Step("Create new employee", async () =>
        {
            await httpClient.AuthAsCto();

            return await scenario.Step(
                "Call 'Users' endpoint to create new user", async () =>
                {
                    var createdUser = await scenario.Step("Create new user", async () =>
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

                        return await httpClient.CreateUser(userToCreate);
                    });

                    return createdUser;
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
                        Name: Guid.CreateVersion7().ToAlphabetic(),
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

        await scenario.Step("Create user specialization", async () =>
        {
            var response = await scenario.Step("Create user specialization", async () =>
            {
                return await httpClient.PostAsJsonAsync(
                    "api/user-specialization",
                    new CreateUserSpecializationInput
                    (
                        UserId: createdUser!.Id,
                        SpecializationTemplateId: createdSpecializationTemplate!.Id,
                        SpecializationSeniorityLevelId: seniorityLevelId,
                        HardSkills:
                        [
                            new HardSkillSeniorityDto
                            {
                                Id = hardSkillIds.FirstOrDefault(),
                                SeniorityLevelId = hardSkillSeniorityLevelId
                            }
                        ]
                    ),
                    _cancellationToken);
            });

            await scenario.Step(
                "Validate response status code",
                () => response.StatusCode.Should().Be(HttpStatusCode.OK));
        });

        await scenario.Step("Check created by specialization user skills snapshot", async () =>
        {
            var userSkillsSnapshots = await scenario.Step("Get user skills snapshots by user id", async () =>
            {
                return await httpClient.GetFromJsonAsync<PageableListOfItems<UserSpecializationTemplateWithSnapshotDto>>(
                    $"api/user-skills-snapshot?userId={createdUser!.Id}&$top=50&$skip=0",
                    _cancellationToken);
            });

            await scenario.Step("Validate response body", () =>
            {
                userSkillsSnapshots.Should().NotBeNull();

                var userSkillsSnapshot = userSkillsSnapshots.Items.FirstOrDefault()
                .SpecializationSnapshots.FirstOrDefault();

                userSkillsSnapshot.SnapshotDate.Should().Be(snapshotDate);
                userSkillsSnapshot.UserId.Should().Be(createdUser!.Id);
                userSkillsSnapshot.UserSpecializations.FirstOrDefault().Should().NotBeNull();
                userSkillsSnapshot.UserSpecializations.Select(x => x.SpecializationTemplateId).FirstOrDefault()
                .Should().Be(createdSpecializationTemplate!.Id);
                userSkillsSnapshot.UserSpecializations.Select(x =>
                x.HardSkills.Select(x => x.Id).FirstOrDefault()).FirstOrDefault()
                .Should().Be(hardSkillIds.FirstOrDefault());
                userSkillsSnapshot.UserSpecializations.Select(x =>
                x.HardSkills.Select(x => x.SeniorityLevelId).FirstOrDefault()).FirstOrDefault()
                .Should().Be(hardSkillSeniorityLevelName);
            });
        });
    }
}
