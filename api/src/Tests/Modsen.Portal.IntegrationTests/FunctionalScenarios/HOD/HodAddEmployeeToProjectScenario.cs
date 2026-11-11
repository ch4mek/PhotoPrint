using System.Net.Http.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
using NScenario;
using Xunit.Abstractions;

namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class HodAddEmployeeToProjectScenario
{
    private readonly FunctionalTestFactory _factory;
    private readonly ITestOutputHelper _outputHelper;

    public HodAddEmployeeToProjectScenario(
        FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _factory = testFactory;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task AddEmployeeToProjectScenario()
    {
        using var httpClient = _factory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(
            outputAdapter,
            scenarioPrefix: "[Head of Department]",
            title: "Head of Department Assign user to project scenario");

        var currentUser = await scenario.Step("Login", async () =>
        {
            await scenario.Step(
                "Call auth endpoint",
                () => httpClient.AuthAsHeadOfDepartment());

            var user = await scenario.Step("Call 'Me' endpoint for getting info about current user", async () =>
            {
                return await httpClient.GetCurrentUser();
            });

            await scenario.Step("Verify info about user", () =>
            {
                user.Should().NotBeNull();
                user.DepartmentId.Should().NotBeNull();
                user.Email.Should().NotBeNullOrEmpty();
                user.Position.Should().Be(PortalRoleNames.HeadOfDepartment);
            });

            return user;
        });

        var employee = await scenario.Step("Get an employee", async () =>
        {
            var availableEmployees = await scenario.Step("Get all available employees", async () =>
            {
                return await httpClient.GetUsersLookup(roles: [PortalRoleNames.Employee], isActive: true);
            });

            var pickedEmployee = await scenario.Step(
                "Pick last Employee",
                () => availableEmployees[^1]);

            var employeeFullName = $"{pickedEmployee.FirstName} {pickedEmployee.LastName}";

            var fullEmployeeInfo = await scenario.Step($"[{employeeFullName}] Get full user info", async () =>
            {
                return await httpClient.GetUserAsync(pickedEmployee.Id);
            });

            await scenario.Step($"[{employeeFullName}] Verify user", () =>
            {
                fullEmployeeInfo.Should().NotBeNull();
                fullEmployeeInfo.Id.Should().Be(pickedEmployee.Id);
                fullEmployeeInfo.FirstName.Should().Be(pickedEmployee.FirstName);
                fullEmployeeInfo.LastName.Should().Be(pickedEmployee.LastName);
                fullEmployeeInfo.Color.Should().Be(pickedEmployee.Color);
                fullEmployeeInfo.AvatarFileId.Should().Be(pickedEmployee.AvatarFileId);
                fullEmployeeInfo.DepartmentId.Should().Be(currentUser.DepartmentId);
            });

            return fullEmployeeInfo!;
        });

        await httpClient.CreateProject(
            new CreateProjectInput
            {
                ClientId = _factory.ModsenClientId!.Value,
                IsBillable = true,
                IsPublic = false,
                Name = "New Project Marketplace"
            });

        var project = await scenario.Step("Get billable project", async () =>
        {
            var projects = await scenario.Step("Get available projects", async () =>
            {
                return await httpClient.GetProjectsSortedLookupAsync();
            });

            var assignments = await scenario.Step("Get user's assignments to projects", async () =>
            {
                return await httpClient.GetUserProjectsLookupAsync(userId: employee.Id);
            });

            var pickedProject = await scenario.Step("Pick project", async () =>
            {
                return projects.FindLast(project => project.Type == PortalProjectTypeNames.Private &&
                    assignments.All(userProject => userProject.ProjectId != project.Id));
            });

            var fullProjectInfo = await scenario.Step($"[{pickedProject!.Name}] Get full project info", async () =>
            {
                return await httpClient.GetProjectByIdAsync(pickedProject.Id);
            });

            await scenario.Step($"[{pickedProject.Name}] Verify project", () =>
            {
                fullProjectInfo.Should().NotBeNull();
                fullProjectInfo.Id.Should().Be(pickedProject.Id);
                fullProjectInfo.Name.Should().Be(pickedProject.Name);
                fullProjectInfo.Color.Should().Be(pickedProject.Color);
                fullProjectInfo.IsPublic.Should().BeFalse();
                fullProjectInfo.IsBillable.Should().BeTrue();
            });

            return fullProjectInfo;
        });

        await scenario.Step("Assign employee to project", async () =>
        {
            var response = await scenario.Step("Call POST endpoint", async () =>
            {
                return await httpClient.CreateProjectAssignmentAsHttpMessageAsync(userId: employee.Id,
                    projectId: project.Id);
            });

            await scenario.Step("Verify if user-project assignment has been created", async () =>
            {
                response.IsSuccessStatusCode.Should().BeTrue();

                var body = await response.Content.ReadFromJsonAsync<UserProjectAssignmentOutput>();
                var assignmentId = body!.Id;

                body.ProjectId.Should().Be(project.Id);
                body.UserId.Should().Be(employee.Id);

                var assignment = await scenario.Step("Get assignment by retrieved Id", async () =>
                {
                    return await httpClient.GetProjectAssignmentAsync(assignmentId);
                });

                await scenario.Step("Verify if retrieved user-project assignment valid", () =>
                {
                    assignment.Id.Should().Be(assignmentId);
                    assignment.ProjectId.Should().Be(project.Id);
                    assignment.UserId.Should().Be(employee.Id);
                });
            });
        });

        await scenario.Step($"[{project.Name}] Check updated project", async () =>
        {
            await httpClient.GetProjectByIdAsync(project.Id);
        });
    }

}
