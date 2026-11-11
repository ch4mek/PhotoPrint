using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.OData;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.UserProjectsController;

[Collection("TimeTrackerCollection")]
public class GetOdpUserProjectsIntegrationTests  : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetOdpUserProjectsIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Theory]
    [InlineData("")]
    [InlineData("?")]
    [InlineData("top=50")]
    [InlineData("skip=0")]
    [InlineData("top=0&skip=50")]
    [InlineData("top=50&skip=-1")]
    [InlineData("top=50&skip=15")]
    [InlineData("top=26&skip=50")]
    [InlineData("top=50&skip=50&filter=Id eq rr")]
    public async Task GetOdpUserProjectsWhenRequestIsInvalidResponseWithBadRequest(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetUserProjects(query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetOdpUserProjectsWhenAuthAsHeadOfDepartmentOfAnotherDepartmentResponseOkWithEmptyList()
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

        var user = await client.CreateUser(userToCreate);

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

        var projectUserResponse = await client.CreateProjectAssignment(user!.Id,createdProject.Id,
            projectUserToCreate);

        projectUserResponse.IsSuccessStatusCode.Should().BeTrue();

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.GetUserProjects($"filter=UserId eq {user.Id}&top=20&skip=0");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PageableListOfItems<OdpProjectUser>>(_cancellationToken);
        body.Should().NotBeNull();
        body.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetOdpUserProjectsWhenAuthAsGroupManagerOfAnotherResourceGroupResponseOkWithEmptyList()
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

        var user = await client.CreateUser(userToCreate);

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

        var projectUserResponse = await client.CreateProjectAssignment(user!.Id, createdProject.Id,
            projectUserToCreate);

        projectUserResponse.IsSuccessStatusCode.Should().BeTrue();

        await client.AuthAsGroupManager();

        // Act
        var response = await client.GetUserProjects($"filter=UserId eq {user.Id}&top=20&skip=0");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PageableListOfItems<OdpProjectUser>>(_cancellationToken);
        body.Should().NotBeNull();
        body.Items.Should().BeEmpty();
    }

    [Theory]
    [MemberData(nameof(InactiveIntervals))]
    public async Task GetOdpProjectUsersWhenAssignIntervalBeforeCurrentDateShouldResponseWithInactiveStatus(DateOnly dateFrom, DateOnly? dateTo)
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

        var user = await client.CreateUser(userToCreate);

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

        var projectUserToCreate = new CreateUserProjectAssignmentInput(dateFrom, dateTo, IsTimeOnProjectBillable: true,
            Fte: 1m, UserOnProjectSeniorityLevelId: "SeniorL2");

        var projectUserResponse = await client.CreateProjectAssignment(user!.Id, createdProject.Id,
            projectUserToCreate);

        projectUserResponse.IsSuccessStatusCode.Should().BeTrue();

        // Act
        var response = await client
            .GetUserProjects($"filter=UserId eq {user.Id} and ProjectId eq {createdProject.Id}&top=20&skip=0");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var projectUsers = await response.Content.ReadFromJsonAsync<PageableListOfItems<OdpProjectUser>>(_cancellationToken);

        projectUsers.Items.First().IsActive.Should().BeFalse();
    }

    public static IEnumerable<object[]> InactiveIntervals =>
        new List<object[]>
        {
            new object []{ DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)) },
            new object []{ DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-100)), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-20)) }
        };
}
