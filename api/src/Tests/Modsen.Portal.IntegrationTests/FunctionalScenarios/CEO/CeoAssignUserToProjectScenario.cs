using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
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
public class CeoAssignUserToProjectScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public CeoAssignUserToProjectScenario(FunctionalTestFactory testFactory, ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task AssignUserToProjectScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[CEO]",
            title: "CEO Assign a User to a Project scenario");

        await scenario.Step("Login", async () =>
        {
            await scenario.Step("Call auth endpoint", async () => await httpClient.AuthAsCeo());

            await scenario.Step("Call 'Me' endpoint for getting info about current user", async () =>
            {
                return await httpClient.GetCurrentUser();
            });
        });

        var createdUser = await scenario.Step("Create new user", async () =>
        {
            return await httpClient.CreateEmployee();
        });

        var project = await scenario.Step("Get project", async () =>
        {
            var projectName = Guid.CreateVersion7().ToAlphabetic();
            await scenario.Step("Create new project", async () =>
            {
                await httpClient.CreateProjectAsHttpMessageAsync(new CreateProjectInput
                {
                    Name = projectName,
                    ClientId = _testFactory.ModsenClientId!.Value,
                    IsBillable = true,
                    IsPublic = false
                });
            });

            var projects = await scenario.Step("Get projects lookup", async () =>
            {
                return await httpClient.GetProjectsLookupAsync(isActive: true, roles: "Employee");
            });

            var project = await scenario.Step($"Find created project {projectName}", () =>
            {
                return projects.FirstOrDefault(x => x.Name == projectName);
            });

            return project;
        });

        await scenario.Step("Assign the project to the user", async () =>
        {
            var response = await scenario.Step("Call POST endpoint", async () =>
            {
                return await httpClient.CreateProjectAssignmentAsHttpMessageAsync(userId: createdUser!.Id,
                    projectId: project!.Id);
            });

            await scenario.Step("Verify if user-project assignment has been created", async () =>
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var body = await response.Content.ReadFromJsonAsync<UserProjectAssignmentOutput>();
                var assignmentId = body!.Id;

                body.ProjectId.Should().Be(project!.Id);
                body.UserId.Should().Be(createdUser!.Id);

                var assignment = await scenario.Step("Get assignment by retrieved Id", async () =>
                {
                    return await httpClient.GetProjectAssignmentAsync(assignmentId);
                });

                await scenario.Step("Verify if retrieved user-project assignment valid", async () =>
                {
                    assignment.Id.Should().Be(assignmentId);
                    assignment.ProjectId.Should().Be(project.Id);
                    assignment.UserId.Should().Be(createdUser.Id);
                });
            });
        });
    }
}
