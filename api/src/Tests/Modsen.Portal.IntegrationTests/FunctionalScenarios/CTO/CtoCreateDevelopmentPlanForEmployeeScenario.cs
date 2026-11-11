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
public class CtoCreateDevelopmentPlanForEmployeeScenario
{
    private readonly FunctionalTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly JsonSerializerOptions _serializerOptions;

    public CtoCreateDevelopmentPlanForEmployeeScenario(
        FunctionalTestFactory factory,
        ITestOutputHelper outputHelper)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _serializerOptions = _factory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task CreateDevelopmentPlanForEmployeeScenario()
    {
        using var httpClient = _factory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(
            outputAdapter,
            scenarioPrefix: "[CTO]",
            title: "Create development plan for employee scenario");

        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var deadlineDate = startDate.AddMonths(6);
        var summary = Guid.CreateVersion7().ToAlphabetic();
        var notes = Guid.CreateVersion7().ToAlphabetic();

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

        var createdEmployeeDevelopmentPlan = await scenario.Step("Create development plan for employee", async () =>
        {
            var response = await scenario.Step("Create contract", async () =>
            {
            return await httpClient.PostAsJsonAsync(
                "api/development-plans",
                new CreateDevelopmentPlanInput
                {
                    StartDate = startDate,
                    DeadlineDate = deadlineDate,
                    Summary = summary,
                    Notes = notes,
                    UserId = createdUser!.Id,
                },
                _cancellationToken);
            });

            await scenario.Step(
                "Validate response status code",
                () => response.StatusCode.Should().Be(HttpStatusCode.OK));

            return await response.Content.ReadFromJsonAsync<DevelopmentPlanOutput>(
                _serializerOptions,
                _cancellationToken);
        });

        await scenario.Step("Check created development plan", async () =>
        {
            var developmentPlan = await scenario.Step("Get user development plan", async () =>
            {
                return await httpClient.GetFromJsonAsync<DevelopmentPlanOutput>(
                    $"api/development-plans/{createdEmployeeDevelopmentPlan!.Id}",
                    _cancellationToken);
            });

            await scenario.Step("Validate response body", () =>
            {
                developmentPlan.Should().NotBeNull();
                developmentPlan.StartDate.Should().Be(startDate);
                developmentPlan.DeadlineDate.Should().Be(deadlineDate);
                developmentPlan.Summary.Should().Be(summary);
                developmentPlan.Notes.Should().Be(notes);
                developmentPlan.UserId.Should().Be(createdUser!.Id);
            });
        });
    }
}
