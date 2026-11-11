using System.Net;
using System.Text.Json;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;

namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.ClientsController;

[Collection("TimeTrackerCollection")]
public class GetClientsIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    public GetClientsIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Theory]
    [InlineData("")]
    [InlineData("?")]
    [InlineData("?top=50")]
    [InlineData("?skip=0")]
    [InlineData("?top=0&skip=50")]
    [InlineData("?top=50&skip=-1")]
    [InlineData("?top=50&skip=15")]
    [InlineData("?top=26&skip=50")]
    [InlineData("?top=50&skip=100&select=id,firstName")]
    [InlineData("?top=50&skip=0&apply=groupby((position), aggregate(id with count as Count))")]
    public async Task GetClientsBySearchPatternWhenRequestIsInvalidResponseWithBadRequestResult(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetClientsWithRawSearchAsHttpMessageAsync(query: query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("?top=10&skip=0")]
    [InlineData("?top=10&skip=50")]
    [InlineData("?top=50&skip=50&filter=contains(name,'Modsen')")]
    [InlineData("?top=50&skip=50&orderby=name")]
    [InlineData("?top=50&skip=50&filter=contains(name,'Modsen')&orderby=name")]
    [InlineData("?top=50&skip=50&filter=Name eq 'Modsen'")]
    [InlineData("?$filter=((IsActive eq false) and (((contains(tolower(Name), tolower(''))))))&$orderby=Name asc&$top=50&$skip=0")]
    [InlineData("?$filter=((IsActive eq true) and (((contains(tolower(Name), tolower(''))))))&$orderby=Name asc&$top=50&$skip=0")]
    [InlineData("?$filter=((IsActive eq true) and (((contains(tolower(Comments), tolower(''))))))&$top=50&$skip=0")]
    public async Task GetClientsBySearchPatternWhenRequestIsValidResponseWithOk(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetClientsWithRawSearchAsHttpMessageAsync(query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("$top=10&$skip=0")]
    [InlineData("$top=10&$skip=50")]
    [InlineData("$top=50&$skip=50&$filter=contains(name,'Modsen')")]
    [InlineData("$top=50&$skip=50&$orderby=name")]
    [InlineData("$top=50&$skip=50&$filter=contains(name,'Modsen')&orderby=name")]
    [InlineData("$top=50&$skip=50&$filter=Name eq 'Modsen'")]
    [InlineData("$$filter=((IsActive eq false) and (((contains(tolower(Name), tolower(''))))))&$orderby=Name asc&$top=50&$skip=0")]
    [InlineData("$$filter=((IsActive eq true) and (((contains(tolower(Name), tolower(''))))))&$orderby=Name asc&$top=50&$skip=0")]
    [InlineData("$$filter=((IsActive eq true) and (((contains(tolower(Comments), tolower(''))))))&$top=50&$skip=0")]
    public async Task GetClientsBySearchPatternWithPostMethodWhenRequestIsValidResponseWithOk(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetClientsWithSearchAsPostMethodAsync(query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("FactTrackedTimeInMinutes", true)]
    [InlineData("BillableTrackedTimeInMinutes", false)]
    [InlineData("Name", true)]
    [InlineData("Name", false)]
    [InlineData("Comments", true)]
    [InlineData("Comments", false)]
    public async Task GetClientsBySearchPatternWhenRequestContainsValidOrderResponseWithExpectedOrder(string orderField, bool isAsc)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var expectedOutput = await CreateTestDataAndGetExpectedOutput(client, isAsc);

        var filter = "((isActive eq true) and " + string.Join(" or ", expectedOutput.Select(x => $"contains(name,'{x.Name}')")) + ")";
        var orderBy = $"{orderField} {(isAsc ? "asc" : "desc")}";

        // Act
        var clients = await client.GetClientsWithSearchAsync(
            filter: filter,
            orderBy: orderBy,
            top: 3,
            skip: 0);

        var clientsItems = clients.Items;

        // Assert
        clientsItems.Should().NotBeNullOrEmpty();
        clientsItems.Should().HaveCount(3);
        clientsItems.Should().BeEquivalentTo(expectedOutput, config =>
        {
            config.Excluding(x => x.Id);
            return config;
        });
    }

    [Theory]
    [InlineData("Modsen")]
    [InlineData("modsen")]
    public async Task GetClientsBySearchPatternWhenRequestContainsFilterByContainsNameResponseWithExpectedClients(string value)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var filter = $"(contains(tolower(Name), tolower('{value}')))";

        // Act
        var clients = await client.GetClientsWithSearchAsync(
            filter: filter);

        // Assert
        clients.Items.Should().NotBeNull();
        clients.Items.Should().OnlyContain(x => x.Name.ToLower().Contains(value.ToLower()));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetClientsBySearchPatternWhenRequestContainsFilterByisActiveResponseWithExpectedClients(
        bool isActive)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var filter = $"(IsActive eq {isActive.ToString().ToLower()})";

        // Act
        var clients = await client.GetClientsWithSearchAsync(filter: filter);

        // Assert
        clients.Items.Should().NotBeNull();
        clients.Items.Should().OnlyContain(x => x.IsActive == isActive);
    }

    [Fact]
    public async Task GetClientsBySearchPatternWhenAuthedAsHeadOfDepartmentResponseWithClientsThatHasProjectsCreatedByThisHeadOrHasPublicProjectsOrHasProjectWithUsersFromThisHeadDepartment()
    {
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var clients = new List<CreateClientOutput>();
        for (var i = 0; i < 5; i++)
        {
            clients.Add((await client.CreateClientAsync())!);
        }

        await client.AuthAsHeadOfDepartment();

        await client.CreateProject(
                clientId: clients[4].Id,
                isBillable: false,
                isPublic: false);

        var expectedClientsIds = new List<int> { clients[0].Id, clients[1].Id, clients[2].Id, clients[3].Id };

        // Act
        var response = await client.GetClientsWithSearchAsync();

        // Assert
        response.Items.Should().NotBeNull();
        response.Items.Should().NotContain(x => expectedClientsIds.Contains(x.Id));
    }

    [Fact]
    public async Task
        GetClientsBySearchPatternWhenAuthedAsGroupManagerResponseWithClientsThatHasProjectsCreatedByHimOrHasPublicProjectsOrHasProjectWithUsersFromHisResourceGroup()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsCeo();
        var gmId = _factory.GroupManagerId!.Value;

        var createdUserFromGroup = await httpClient.CreateEmployee(resourceGroupId: 1);
        var userFromGroupId = createdUserFromGroup!.Id;

        var createdUserNotFromGroup = await httpClient.CreateEmployee(
            departmentId: "Java",
            resourceGroupId: 2);
        var userNotFromGroupId = createdUserNotFromGroup!.Id;

        var clientWithProjectWithUserFromSameGroup = await httpClient.CreateClientAsync();
        var clientWithProjectWithUserFromOtherGroup = await httpClient.CreateClientAsync();
        var clientWithPublicProject = await httpClient.CreateClientAsync();
        var clientWithProjectWithGroupManager = await httpClient.CreateClientAsync();

        var projectWithUserFromSameGroup = await httpClient.CreateProject(
            clientId: clientWithProjectWithUserFromSameGroup!.Id,
            isPublic: false, isBillable: false);
        var projectWithUserFromOtherGroup = await httpClient.CreateProject(
            clientId: clientWithProjectWithUserFromOtherGroup!.Id,
            isPublic: false, isBillable: false);
        await httpClient.CreateProject(
            clientId: clientWithPublicProject!.Id,
            isPublic: true, isBillable: false);
        var projectWithGroupManager = await httpClient.CreateProject(
            clientId: clientWithProjectWithGroupManager!.Id,
            isPublic: false, isBillable: false);

        await httpClient.CreateProjectAssignment(
            userId: userFromGroupId,
            projectId: projectWithUserFromSameGroup!.Id);

        await httpClient.CreateProjectAssignment(
            userId: userNotFromGroupId,
            projectId: projectWithUserFromOtherGroup!.Id);

        await httpClient.CreateProjectAssignment(
            userId: gmId,
            projectId: projectWithGroupManager!.Id);

        await httpClient.AuthAs(EmailConstants.GroupManagerEmail);

        // Act
        var response = await httpClient.GetClientsWithSearchAsync(top: 50);

        // Assert
        response.Items.Should().NotBeNull();

        var expectedIds = new[]
        {
            clientWithProjectWithUserFromSameGroup.Id,
            clientWithPublicProject.Id,
            clientWithProjectWithGroupManager.Id
        };

        expectedIds.All(id => response.Items.Any(client => client.Id == id)).Should().BeTrue();
        response.Items.Should().NotContain(client => client.Id == clientWithProjectWithUserFromOtherGroup.Id);
    }

    [Theory]
    [InlineData(EmailConstants.CeoEmail)]
    [InlineData(EmailConstants.CtoEmail)]
    public async Task GetClientsWhenAuthedAsCtoOrCeoShouldResponseWithClientsWithTrackedTime(string currentUserEmail)
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        await client.AuthAs(currentUserEmail);

        var response = await client.GetClientsWithSearchAsHttpMessageAsync(top: 10);

        var actualJson = await response.Content.ReadAsStringAsync(_factory.CancellationToken);

        // Assert
        JsonDocument.Parse(actualJson).RootElement.TryGetProperty("items", out var itemsArray);
        itemsArray[0].TryGetProperty("id", out _).Should().BeTrue();
        itemsArray[0].TryGetProperty("name", out _).Should().BeTrue();
        itemsArray[0].TryGetProperty("color", out _).Should().BeTrue();
        itemsArray[0].TryGetProperty("comments", out _).Should().BeTrue();
        itemsArray[0].TryGetProperty("factTrackedTimeInMinutes", out _).Should().BeTrue();
        itemsArray[0].TryGetProperty("billableTrackedTimeInMinutes", out _).Should().BeTrue();
    }

    [Fact]
    public async Task GetClientsWhenAuthedAsHeadOfDepartmentShouldResponseWithClientsWithNullTrackedTime()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var expectedClient = (await CreateTestDataAndGetExpectedOutput(client, isAsc: true)).First();

        var filter = $"(contains(tolower(Name), tolower('{expectedClient.Name}')))";

        // Act
        await client.AuthAsHeadOfDepartment();

        var clients = await client.GetClientsWithSearchAsync(
            filter: filter,
            top: 10);

        // Assert
        clients.Items.Should().Satisfy(x => x.FactTrackedTimeInMinutes == null && x.BillableTrackedTimeInMinutes == null);
    }

    private async Task<List<ClientOutput>> CreateTestDataAndGetExpectedOutput(HttpClient client, bool isAsc)
    {
        var userId = _factory.EmployeeUserId!.Value;
        var clientsForCreating = new List<CreateClientInput>
        {
            new()
            {
                Name = Guid.CreateVersion7().ToAlphabetic(),
                Color = "#000000",
                Comments = "AAAAAAAAA",
                RegisteredLocation = 1
            },
            new()
            {
                Name = Guid.CreateVersion7().ToAlphabetic(),
                Color = "#000000",
                Comments = "NNNNNNNNN",
                RegisteredLocation = 1
            },
            new()
            {
                Name = Guid.CreateVersion7().ToAlphabetic(),
                Color = "#000000",
                Comments = "ZZZZZZZ",
                RegisteredLocation = 1
            }
        };

        var createdClients = new List<CreateClientOutput>();
        foreach (var createClientInput in clientsForCreating)
        {
            var createdClient = await client.CreateClientAsync(createClientInput);
            createdClients.Add(createdClient!);
        }

        var projects = new List<ProjectOutput>();
        foreach (var createdClientOutput in createdClients)
        {
            var createdProject = await client.CreateProject(
                clientId: createdClientOutput.Id,
                isBillable: false);

            await client.CreateProjectAssignment(
                userId: userId,
                projectId: createdProject!.Id);

            projects.Add(createdProject);
        }

        var firstTrack = await client.TrackFactAsync(projectId: projects[0].Id, userId: userId, trackedInMinutes: 10);
        var secondTrack = await client.TrackFactAsync(projectId: projects[1].Id, userId: userId, trackedInMinutes: 20);
        var thirdTrack = await client.TrackFactAsync(projectId: projects[2].Id, userId: userId, trackedInMinutes: 30);

        var expectedClients = new List<ClientOutput>
        {
            new()
            {
                Name = clientsForCreating[0].Name,
                Comments = clientsForCreating[0].Comments,
                Color = clientsForCreating[0].Color,
                IsActive = true,
                IsProjectExist = true,
                HasActiveProject = true,
                FactTrackedTimeInMinutes = firstTrack.FactTrackedInMinutes,
                BillableTrackedTimeInMinutes = 0,
                RegisteredLocation = "Belarus"
            },
            new()
            {
                Name = clientsForCreating[1].Name,
                Comments = clientsForCreating[1].Comments,
                Color = clientsForCreating[1].Color,
                IsActive = true,
                IsProjectExist = true,
                HasActiveProject = true,
                FactTrackedTimeInMinutes = secondTrack.FactTrackedInMinutes,
                BillableTrackedTimeInMinutes = 0,
                RegisteredLocation = "Belarus"
            },
            new()
            {
                Name = clientsForCreating[2].Name,
                Comments = clientsForCreating[2].Comments,
                Color = clientsForCreating[2].Color,
                IsActive = true,
                IsProjectExist = true,
                HasActiveProject = true,
                FactTrackedTimeInMinutes = thirdTrack.FactTrackedInMinutes,
                BillableTrackedTimeInMinutes = 0,
                RegisteredLocation = "Belarus"
            }
        };

        if (!isAsc)
        {
            expectedClients.Reverse();
        }

        return expectedClients;
    }
}
