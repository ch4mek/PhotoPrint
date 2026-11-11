using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Requests.RequestDto;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios.Teamboard;

[Collection("FuncScenarioCollection")]
public class EmployeeDisplaySalaryScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public EmployeeDisplaySalaryScenario(FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task DisplaySalaryScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee Display user salary scenario");

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

        await scenario.Step("Create user salary", async () =>
        {
            var input = new CreateSalaryDto(createdUser!.Id, new DateOnly(2024, 1, 1), 100, null);

            await httpClient.CreateUserSalary(input);
        });

        await scenario.Step("Display user salary", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAs(createdUser!.Email);
            });

            var salary = await scenario.Step("Get user salary", async () =>
            {
                return await httpClient
                    .GetSalary(createdUser!.Id)
                    .WithResponseType<PageableListOfItems<UserSalaryDetailsDto>>();
            });

            await scenario.Step("Verify user personal info", () =>
            {
                salary.Items.First().SalaryUsd.Should().Be(100);
            });
        });
    }
}
