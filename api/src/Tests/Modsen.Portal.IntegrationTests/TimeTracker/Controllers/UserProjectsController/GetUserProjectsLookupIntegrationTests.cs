using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Identity.Controllers.Contracts.Output;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;

namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.UserProjectsController;

[Collection("TimeTrackerCollection")]
public class GetUserProjectsLookupIntegrationTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetUserProjectsLookupIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetUserProjectsLookupWhenUserDoesNotExistResponseNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = int.MaxValue;

        // Act
        var response = await client.GetUserProjects(userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUserProjectsLookupWhenRequestIsValidShouldReturnUserProjects()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = TimeTrackerInputFaker.FakeCreateUserInput();
        var user = await client.CreateUser(userToCreate);

        // Act
        var response = await client.GetUserProjects(user!.Id);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var userProjects = await response.Content.ReadFromJsonAsync<List<UserProjectLookupOutput>>(
            _jsonOptions, _cancellationToken);

        userProjects.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUserProjectsLookupWhenAuthAsHeadOfDepartmentOfAnotherDepartmentResponseWithNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = TimeTrackerInputFaker.FakeCreateUserInput(department: "Js");
        var projectToCreate = TimeTrackerInputFaker.FakeCreateProjectInput(_factory.ModsenClientId!.Value);
        var projectUserToCreate = new CreateUserProjectAssignmentInput(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            DateOnly.FromDateTime(DateTime.UtcNow), IsTimeOnProjectBillable: true, Fte: 1m, UserOnProjectSeniorityLevelId: "SeniorL2");


        (var user, _) = await CreateUserAndProjectAndUserProjectAssignment(client, userToCreate, projectToCreate, projectUserToCreate);
        //var user = await client.CreateUser(userToCreate);

        //var createdProjectResponse = await client.CreateProjectAsHttpResponse(projectToCreate);
        //var createdProject = await createdProjectResponse.Content.ReadFromJsonAsync<ProjectOutput>(_cancellationToken);


        //var projectUserResponse = await client.CreateProjectAssignment(user!.Id, createdProject.Id,
        //    projectUserToCreate);

        //projectUserResponse.IsSuccessStatusCode.Should().BeTrue();

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.GetUserProjects(user.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetUserProjectsLookupWhenAuthAsGroupManagerOfAnotherResourceGroupResponseWithNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = TimeTrackerInputFaker.FakeCreateUserInput();
        var projectToCreate = TimeTrackerInputFaker.FakeCreateProjectInput(_factory.ModsenClientId!.Value);
        var projectUserToCreate = new CreateUserProjectAssignmentInput(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            DateOnly.FromDateTime(DateTime.UtcNow), IsTimeOnProjectBillable: true, Fte: 1m, UserOnProjectSeniorityLevelId: "SeniorL2");

        (var user, _) = await CreateUserAndProjectAndUserProjectAssignment(client, userToCreate, projectToCreate, projectUserToCreate);

        await client.AuthAsGroupManager();

        // Act
        var response = await client.GetUserProjects(user.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetUserProjectsLookupWhenRoleIsEmployeeReturnUserProjectsWithoutIsTimeOnProjectBillableAndClientNameFields()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = TimeTrackerInputFaker.FakeCreateUserInput();
        var projectToCreate = TimeTrackerInputFaker.FakeCreateProjectInput(_factory.ModsenClientId!.Value);
        var projectUserToCreate = new CreateUserProjectAssignmentInput(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            DateOnly.FromDateTime(DateTime.UtcNow), IsTimeOnProjectBillable: true, Fte: 1m, UserOnProjectSeniorityLevelId: "SeniorL2");

        (var user, var createdProject) =
            await CreateUserAndProjectAndUserProjectAssignment(client, userToCreate, projectToCreate, projectUserToCreate);

        await client.AuthAs(user.Email);

        // Act
        var response = await client.GetUserProjects(user.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var userProjects = await response.Content.ReadFromJsonAsync<List<UserProjectLookupOutput>>(
                    _jsonOptions, _cancellationToken);

        userProjects.Should().NotBeNull();
        userProjects[0].ProjectId.Should().Be(createdProject.Id);
        userProjects[0].IsTimeOnProjectBillable.Should().BeNull();
        userProjects[0].ClientName.Should().BeNull();
    }

    [Fact]
    public async Task GetUserProjectsLookupWhenRoleIsGroupManagerReturnUserProjectsWithIsTimeOnProjectBillableAndClientNameFields()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = TimeTrackerInputFaker.FakeCreateUserInput(position: PortalRoleNames.GroupManager);
        var projectToCreate = TimeTrackerInputFaker.FakeCreateProjectInput(_factory.ModsenClientId!.Value);
        var projectUserToCreate = new CreateUserProjectAssignmentInput(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            DateOnly.FromDateTime(DateTime.UtcNow), IsTimeOnProjectBillable: true, Fte: 1m, UserOnProjectSeniorityLevelId: "SeniorL2");

        (var user, var createdProject) =
            await CreateUserAndProjectAndUserProjectAssignment(client, userToCreate, projectToCreate, projectUserToCreate);
        await client.AuthAs(user.Email);

        // Act
        var response = await client.GetUserProjects(user.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var userProjects = await response.Content.ReadFromJsonAsync<List<UserProjectLookupOutput>>(
                    _jsonOptions, _cancellationToken);

        userProjects.Should().NotBeNull();
        userProjects[0].ProjectId.Should().Be(createdProject.Id);
        userProjects[0].IsTimeOnProjectBillable.Should().Be(projectUserToCreate.IsTimeOnProjectBillable);
        userProjects[0].ClientName.Should().NotBeNull();
    }

    private async Task<(UserOutput, ProjectOutput)> CreateUserAndProjectAndUserProjectAssignment(HttpClient client,
        CreateUserInput userToCreate, CreateProjectInput projectToCreate,
        CreateUserProjectAssignmentInput projectUserToCreate)
    {
        var user = await client.CreateUser(userToCreate);

        var createdProjectResponse = await client.CreateProjectAsHttpResponse(projectToCreate);
        var createdProject = await createdProjectResponse.Content.ReadFromJsonAsync<ProjectOutput>(_cancellationToken);

        var projectUserResponse = await client.CreateProjectAssignment(user!.Id, createdProject!.Id, projectUserToCreate);
        projectUserResponse.IsSuccessStatusCode.Should().BeTrue();

        return (user, createdProject);
    }
}
