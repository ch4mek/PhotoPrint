using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Identity.Controllers.Contracts.Output;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using NScenario;
using Xunit.Abstractions;

namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class CtoUpdateSoftSkillForEmployeeScenario
{
    private readonly FunctionalTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly JsonSerializerOptions _serializerOptions;

    public CtoUpdateSoftSkillForEmployeeScenario(
        FunctionalTestFactory factory,
        ITestOutputHelper outputHelper)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _serializerOptions = _factory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task UpdateEmployeeSoftSkillScenario()
    {
        using var httpClient = _factory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(
            outputAdapter,
            scenarioPrefix: "[CTO]",
            title: "CTO Update soft skill for employee scenario");

        var softSkillId = 3;
        var seniorityLevelId = "Lead";

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

        await scenario.Step("Update user soft skill", async () =>
        {
            var response = await scenario.Step("Update soft skill", async () =>
            {
                return await httpClient.PostAsJsonAsync(
                    $"api/users/{createdUser!.Id}/soft-skills",
                    new CreateUserSoftSkillsSnapshotInput
                    {
                        SnapshotDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        UserSoftSkills = new List<UpdateUserSoftSkillInput>
                        {
                            new()
                            {
                                SoftSkillId = softSkillId,
                                SeniorityLevelId = seniorityLevelId
                            },
                        }
                    },
                    _cancellationToken);
            });

            await scenario.Step(
                "Validate response status code",
                () => response.StatusCode.Should().Be(HttpStatusCode.OK));
        });

        await scenario.Step("Check updated soft skill", async () =>
        {
            var softSkills = await scenario.Step("Get assigned user's soft skills", async () =>
            {
                return await httpClient.GetFromJsonAsync<List<AssignedSoftSkillOutput>>(
                    $"api/users/{createdUser!.Id}/soft-skills/assigned",
                    _cancellationToken);
            });

            await scenario.Step("Validate response body", () =>
            {
                softSkills.Should().NotBeNull();
                softSkills.FirstOrDefault(x => x.SoftSkillId == softSkillId).Should().NotBeNull();
                softSkills.First(x => x.SoftSkillId == softSkillId).SeniorityLevelId.Should().Be(seniorityLevelId);
            });
        });
    }
}
