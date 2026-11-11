using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios.Teamboard;

[Collection("FuncScenarioCollection")]
public class EmployeeDisplayProjectsScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public EmployeeDisplayProjectsScenario(FunctionalTestFactory testFactory,
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
            title: "Employee Display user projects scenario");

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

        var createdProject = await scenario.Step("Create project", async () =>
        {
            var input = new CreateProjectInput
            {
                ClientId = _testFactory.ModsenClientId!.Value,
                Color = "#ffffff",
                ManagerId = null,
                Name = Guid.CreateVersion7().ToAlphabetic(),
                IsBillable = false,
                IsPublic = false
            };

            return await httpClient.CreateProjectAsHttpResponse(input)
                .WithResponseType<ProjectOutput>();
        });

        await scenario.Step("Assign user to project", async () =>
        {
            var input = new CreateUserProjectAssignmentInput(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                DateOnly.FromDateTime(DateTime.UtcNow), IsTimeOnProjectBillable: true, Fte: 1m, UserOnProjectSeniorityLevelId: "SeniorL2");

            return await httpClient.CreateProjectAssignment(createdUser!.Id, createdProject.Id, input);
        });

        await scenario.Step("Display user projects", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAs(createdUser!.Email);
            });

            var projects = await scenario.Step("Get user projects", async () =>
            {
                return await httpClient
                    .GetUserProjects(createdUser!.Id)
                    .WithResponseType<List<UserProjectLookupOutput>>();
            });

            await scenario.Step("Verify user projects", () =>
            {
                projects.First().ProjectId.Should().Be(createdProject.Id);
            });
        });
    }
}
