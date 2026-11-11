using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class CeoCreateUpdateAndDeactivateProjectScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public CeoCreateUpdateAndDeactivateProjectScenario(
        FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task CreateUpdateAndDeactivateProjectScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[CEO]",
            title: "CEO create, update and deactivate project scenario");

        await scenario.Step("Login", async () =>
        {
            await scenario.Step("Call auth endpoint", async () => await httpClient.AuthAsCeo());

            await scenario.Step(
                "Call 'Me' endpoint for getting info about current user",
                async () =>
                {
                    return await httpClient.GetCurrentUser();
                });
        });

        var (createdProject, userProjectAssignmentIds) = await scenario.Step("Create project", async () =>
        {
            var clients = await scenario.Step("Get clients lookup", async () =>
            {
                return await httpClient.GetClientsLookupAsync();
            });

            var users = await scenario.Step("Get users lookup", async () =>
            {
                return await httpClient.GetUsersLookup(roles: ["Employee"]);
            });

            var projectClient = clients![0];
            var projectUsers = users.Take(2);

            var createProjectModel = new CreateProjectInput
            {
                ClientId = projectClient.Id,
                IsBillable = true,
                IsPublic = false,
                Name = "CreateUpdateAndDeactivateProjectScenario",
                Color = "#8A8E3B"
            };

            var createdProject = await scenario.Step("Create new project", async () =>
            {
                return await httpClient.CreateProject(createProjectModel);
            });

            await scenario.Step("Check if project was created", async () =>
            {
                var project = await httpClient.GetProjectByIdAsync(createdProject!.Id);

                project.Id.Should().Be(createdProject.Id);
                project.ClientId.Should().Be(createdProject.ClientId);
                project.Name.Should().Be(createdProject.Name);
                project.Color.Should().Be(createdProject.Color);
            });

            var userProjectAssignmentIds = await scenario.Step("Assign the project to the 3 users", async () =>
            {
                var userProjectAssignmentIds = new List<int>();
                foreach (var user in projectUsers)
                {
                    var response = await scenario.Step("Call POST endpoint", async () =>
                    {
                        return await httpClient.CreateProjectAssignmentAsHttpMessageAsync(userId: user.Id,
                            projectId: createdProject!.Id);
                    });

                    await scenario.Step("Verify if user-project assignment has been created", async () =>
                    {
                        response.StatusCode.Should().Be(HttpStatusCode.OK);

                        var body = await response.Content.ReadFromJsonAsync<UserProjectAssignmentOutput>();
                        var assignmentId = body!.Id;
                        userProjectAssignmentIds.Add(assignmentId);

                        body.ProjectId.Should().Be(createdProject!.Id);
                        body.UserId.Should().Be(user.Id);

                        var assignment = await scenario.Step("Get assignment by retrieved Id", async () =>
                        {
                            return await httpClient.GetProjectAssignmentAsync(assignmentId);
                        });

                        await scenario.Step("Verify if retrieved user-project assignment valid", async () =>
                        {
                            assignment.Id.Should().Be(assignmentId);
                            assignment.ProjectId.Should().Be(createdProject.Id);
                            assignment.UserId.Should().Be(user.Id);
                        });
                    });
                }
                return userProjectAssignmentIds;
            });

            return (createdProject, userProjectAssignmentIds);
        });

        var updatedProject = await scenario.Step("Update project", async () =>
        {
            var updateProjectModel = await scenario.Step("Change project information", () =>
            {
                return new UpdateProjectInput
                {
                    Id = createdProject!.Id,
                    ClientId = createdProject.ClientId,
                    Name = "New Cool project",
                    Color = "#8A8A5A",
                    IsBillable = createdProject.IsBillable,
                    IsPublic = createdProject.IsPublic
                };
            });

            await scenario.Step("Call PUT project endpoint", async () =>
            {
                await httpClient.UpdateProjectAsync(updateProjectModel);
            });

            await scenario.Step("Check if project was updated", async () =>
            {
                var project = await httpClient.GetProjectByIdAsync(createdProject!.Id);

                project.Id.Should().Be(createdProject.Id);
                project.ClientId.Should().Be(createdProject.ClientId);
                project.Name.Should().Be(updateProjectModel.Name);
                project.Color.Should().Be(updateProjectModel.Color);
            });

            return updateProjectModel;
        });

        await scenario.Step("Deactivate project", async () =>
        {
            await scenario.Step("Reschedule user-projects assignments", async () =>
            {
                var newDateFrom = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5);
                var newDateTo = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5);
                var fte = 1;
                var seniority = "SeniorL3";
                var billable = true;
                var userId = 1;
                var projectId = 1;
                foreach (var assignmentId in userProjectAssignmentIds)
                {
                    await httpClient.UpdateProjectAssignmentAsHttpMessageAsync(new UpdateUserProjectAssignmentInput(
                        assignmentId,
                        userId,
                        projectId,
                        billable,
                        fte,
                        seniority,
                        newDateFrom, newDateTo));
                }
            });

            await scenario.Step("Deactivate project", async () =>
            {
                await httpClient.SetProjectInactiveAsHttpMessageAsync(createdProject!.Id);
            });

            await scenario.Step("Check if project was deactivated", async () =>
            {
                var project = await httpClient.GetProjectByIdAsync(createdProject!.Id);

                project.Id.Should().Be(updatedProject.Id);
                project.ClientId.Should().Be(updatedProject.ClientId);
                project.Name.Should().Be(updatedProject.Name);
                project.Color.Should().Be(updatedProject.Color);
            });
        });
    }

}
