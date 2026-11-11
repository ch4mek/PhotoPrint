using System.Net;
using System.Net.Http.Json;
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
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.UserProjectsController;

[Collection("TimeTrackerCollection")]
public class CreateUserProjectAssignmentIntegrationTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CreateUserProjectAssignmentIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

        [Fact]
    public async Task CreateUserProjectAssignmentWhenAuthAsHeadOfDepartmentForUserOfAnotherDepartmentResponseWithForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = new CreateUserInput
        {
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            Color = "#ffffff",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "Js",
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var projectToCreate = new CreateProjectInput
        {
            ClientId = _factory.ModsenClientId!.Value,
            Color = "#ffffff",
            ManagerId = null,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            IsBillable = false,
            IsPublic = false
        };

        var createdProjectResponse = await client.CreateProjectAsHttpResponse(projectToCreate);

        var createdProject = await createdProjectResponse.Content.ReadFromJsonAsync<ProjectOutput>(_cancellationToken);

        var user = await client.CreateUser(userToCreate);

        var projectUserToCreate = new CreateUserProjectAssignmentInput(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            DateOnly.FromDateTime(DateTime.UtcNow), IsTimeOnProjectBillable: true, Fte: 1m, UserOnProjectSeniorityLevelId: "SeniorL2");

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.CreateProjectAssignment(user!.Id, createdProject.Id,
            projectUserToCreate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateUserProjectAssignmentWhenAuthAsGroupManagerForUserOfAnotherResourceGroupResponseWithForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = new CreateUserInput
        {
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            Color = "#ffffff",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "Net",
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var projectToCreate = new CreateProjectInput
        {
            ClientId = _factory.ModsenClientId!.Value,
            Color = "#ffffff",
            ManagerId = null,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            IsBillable = false,
            IsPublic = false
        };

        var createdProjectResponse = await client.CreateProjectAsHttpResponse(projectToCreate);

        var createdProject = await createdProjectResponse.Content.ReadFromJsonAsync<ProjectOutput>(_cancellationToken);

        var user = await client.CreateUser(userToCreate);

        var projectUserToCreate = new CreateUserProjectAssignmentInput(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            DateOnly.FromDateTime(DateTime.UtcNow), IsTimeOnProjectBillable: true, Fte: 1m, UserOnProjectSeniorityLevelId: "SeniorL2");

        await client.AuthAsGroupManager();

        // Act
        var response = await client.CreateProjectAssignment(user!.Id, createdProject.Id,
            projectUserToCreate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateUserProjectAssignmentWhenRequestIsValidResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = _factory.EmployeeUserId!.Value;

        var projectToCreate = new CreateProjectInput
        {
            ClientId = _factory.ModsenClientId!.Value,
            Color = "#ffffff",
            ManagerId = null,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            IsBillable = false,
            IsPublic = false
        };

        var createdProjectResponse = await client.CreateProjectAsHttpResponse(projectToCreate);

        var createdProject = await createdProjectResponse.Content.ReadFromJsonAsync<ProjectOutput>(_cancellationToken);

        var projectUserToCreate = new CreateUserProjectAssignmentInput(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            DateOnly.FromDateTime(DateTime.UtcNow), IsTimeOnProjectBillable: true, Fte: 1m, UserOnProjectSeniorityLevelId: "SeniorL2");

        // Act
        var response = await client.CreateProjectAssignment(userId, createdProject.Id,
            projectUserToCreate);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
    }
}
