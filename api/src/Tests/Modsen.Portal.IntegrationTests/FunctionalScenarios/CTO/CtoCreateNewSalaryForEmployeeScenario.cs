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
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Requests.RequestDto;
using NScenario;
using Xunit.Abstractions;

namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class CtoCreateNewSalaryForEmployeeScenario
{
    private readonly FunctionalTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly JsonSerializerOptions _serializerOptions;

    public CtoCreateNewSalaryForEmployeeScenario(
        FunctionalTestFactory factory,
        ITestOutputHelper outputHelper)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _serializerOptions = _factory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task CreateNewSalaryForEmployeeScenario()
    {
        using var httpClient = _factory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(
            outputAdapter,
            scenarioPrefix: "[CTO]",
            title: "CTO Create new salary for employee scenario");

        var date = DateOnly.FromDateTime(DateTime.Now);
        var newDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(3));
        var salaryUsd = 1000m;
        var newSalaryUsd = 1500m;

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

        await scenario.Step("Create salary for employee", async () =>
        {
            var response = await scenario.Step("Create salary", async () =>
            {
                return await httpClient.PostAsJsonAsync(
                    "api/user-salary",
                    new CreateSalaryDto
                    (
                        UserId: createdUser!.Id,
                        Date: date,
                        SalaryUsd: salaryUsd,
                        null
                    ),
                    _cancellationToken);
            });

            await scenario.Step(
                "Validate response status code",
                () => response.StatusCode.Should().Be(HttpStatusCode.OK));
        });

        await scenario.Step("Create new salary for employee", async () =>
        {
            var response = await scenario.Step("Create salary", async () =>
            {
                return await httpClient.PostAsJsonAsync(
                    "api/user-salary",
                    new CreateSalaryDto
                    (
                        UserId: createdUser!.Id,
                        Date: newDate,
                        SalaryUsd: newSalaryUsd,
                        null
                    ),
                    _cancellationToken);
            });

            await scenario.Step(
                "Validate response status code",
                () => response.StatusCode.Should().Be(HttpStatusCode.OK));

            return await response.Content.ReadFromJsonAsync<CreateUserSalaryOutput>(
                _serializerOptions,
                _cancellationToken);
        });

        await scenario.Step("Check new salary", async () =>
        {
            var salaries = await scenario.Step("Get user salaries", async () =>
            {
                return await httpClient.GetFromJsonAsync<PageableListOfItems<UserSalaryDetailsDto>>(
                    $"api/users/{createdUser!.Id}/salaries?$top=50&$skip=0",
                    _cancellationToken);
            });

            await scenario.Step("Validate response body", () =>
            {
                salaries.Items.Should().NotBeNull();
                salaries.Items.Count.Should().Be(2);
                salaries.Items.FirstOrDefault(x => x.SalaryUsd == newSalaryUsd).Should().NotBeNull();
            });
        });
    }
}
