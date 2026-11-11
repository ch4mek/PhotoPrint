using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
using NScenario;
using Xunit.Abstractions;

namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class EmployeeDisplayProjectsInFilterAccordingToSelectedClientScenario
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public EmployeeDisplayProjectsInFilterAccordingToSelectedClientScenario(FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _serializerOptions = _testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task EmployeeDisplayProjectsInFilterAccordingToSelectedClient()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee Display projects in a filter according to the selected Client scenario");

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
            return await scenario.Step(
                "Call 'Users' endpoint to create new user", async () =>
                {
                    var createdUser = await scenario.Step("Create new user", async () =>
                    {
                        return await httpClient.CreateEmployee(departmentId: "PMO");
                    });

                    return createdUser;
                });
        });

        var createdProject = await scenario.Step("Create project", async () =>
        {
            var project = await scenario.Step("[Projects] Call POST endpoint", async () =>
            {
                var body = new CreateProjectInput
                {
                    ClientId = _testFactory.ModsenClientId!.Value,
                    Color = null,
                    IsBillable = false,
                    IsPublic = false,
                    Name = $"{Guid.CreateVersion7().ToAlphabetic()}",
                    ManagerId = createdUser!.Id
                };

                var response = await httpClient.CreateProjectAsHttpMessageAsync(body);
                if (!response.IsSuccessStatusCode)
                {
                    var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>(_cancellationToken);
                    exception?.ErrorInfo.Should().BeNullOrEmpty();
                }

                return await response.Content.ReadFromJsonAsync<ProjectOutput>(_serializerOptions, _cancellationToken);
            });

            await scenario.Step($"[{project!.Name}] Verify response", () =>
            {
                project.Should().NotBeNull();
            });

            return project;
        });

        await scenario.Step("Login", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAs(createdUser!.Email);
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
                user.DepartmentId.Should().NotBeNull();
                user.Email.Should().NotBeNullOrEmpty();
            });

            return user;
        });

        await scenario.Step("Get projects from filter for timesheets", async () =>
        {
            var lookupProjectsFromFilter = await httpClient.GetProjectsByClientIdAsync(createdProject.ClientId);

            await scenario.Step("Verify projects responses", () =>
            {
                return lookupProjectsFromFilter.Select(x => x.Id).Contains(createdProject.Id);
            });
        });
    }
}
