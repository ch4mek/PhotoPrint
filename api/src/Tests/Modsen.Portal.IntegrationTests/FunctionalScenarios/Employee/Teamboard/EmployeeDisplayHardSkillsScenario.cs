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
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests.RequestDto;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios.Teamboard;

[Collection("EmployeeTeamboardFuncScenarioCollection")]
public class EmployeeDisplayHardSkillsScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public EmployeeDisplayHardSkillsScenario(FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task DisplayHardSkillsScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee Display user hard skills scenario");

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

        const string specializationSeniorityLevel = "JuniorL2";
        const string specializationSeniorityName = "Junior =";
        const string hardSkillSeniorityLevel = "Middle";

        await scenario.Step("Create user hard skill", async () =>
        {
            var createdSpecializationTemplate = await scenario.Step("Create specialization template", async () =>
            {
                var departmentIds = new List<string>();
                var hardSkillIds = new List<int> { 1 };

                var specializationToCreate = new CreateSpecializationTemplateInput(
                    Name: Guid.CreateVersion7().ToAlphabetic(),
                    departmentIds,
                    hardSkillIds
                );

                return await httpClient.CreateSpecializationTemaplate(specializationToCreate)
                    .WithResponseType<SpecializationTemplateOutput>();
            });

            await scenario.Step("Create hard skill", async () =>
            {
                var createHardSkillInput = new CreateHardSkillInput("Hard", "CID");

                return await httpClient.CreateHardSkills(createHardSkillInput).WithResponseType<Portal.TeamBoard.Contract.Contract.Output.HardSkillOutput>();
            });

            await scenario.Step("Create user specialization", async () =>
            {
                var hardSkills = new HardSkillSeniorityDto[] {
                    new() { Id = 1, SeniorityLevelId = hardSkillSeniorityLevel }
                };
                var userSpecializationToCreate = new CreateUserSpecializationInput(
                    createdUser!.Id,
                    createdSpecializationTemplate.Id,
                    SpecializationSeniorityLevelId: specializationSeniorityLevel,
                    hardSkills
                );

                return await httpClient
                    .CreateUserSpecialization(userSpecializationToCreate)
                    .WithResponseType<UserSpecializationCoreDto>();
            });

            await scenario.Step("Create user snapshot", async () =>
            {
                var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10));
                var createUserSnapshotInput = new CreateUserSnapshotInput(createdUser.Id, date,
                [
                    new CreateUserSpecializationDto
                    {
                        SpecializationTemplateId = 1,
                        HardSkills = [new HardSkillSeniorityDto { Id = 1, SeniorityLevelId = "Senior" }]
                    }
                ]);

                await httpClient.CreateUserSkillSnapshot(createUserSnapshotInput);
            });
        });

        await scenario.Step("Display user hard skills", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAs(createdUser!.Email);
            });

            var hardSkills = await scenario.Step("Get user hard skills", async () =>
            {
                return await httpClient
                    .GetUserSkillsSnapshot(createdUser!.Id, 10, 0)
                    .WithResponseType<PageableListOfItems<UserSpecializationTemplateWithSnapshotDto>>();
            });

            await scenario.Step("Verify user hard skills", () =>
            {
                hardSkills.Items.First().SeniorityLevelName.Should().Be(specializationSeniorityName);
            });
        });
    }
}
