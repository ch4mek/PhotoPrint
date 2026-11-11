using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Finance;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Finance.Controllers.Contracts.Input;
using Modsen.Portal.Finance.Controllers.Contracts.Output;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Finance;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;

namespace Modsen.Portal.IntegrationTests.Finance.ProjectRates;

[Collection("GeneralCollection")]
public class ProjectRatesIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public ProjectRatesIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task CreateRateWhenCannotManageProjectRatesResponseWith403()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();
        var input = new CreateProjectRateInput(1000000, 1, 1, CurrencyCodes.USDollar, new DateOnly(2025, 1, 1));

        // Act
        var response = await client.CreateProjectRateAsResponseMessageAsync(input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateRateWhenUserDoesNotExistResponseWithNotFoundResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var input = new CreateProjectRateInput(1000000, 1, 1, CurrencyCodes.USDollar, new DateOnly(2025, 1, 1));

        // Act
        var response = await client.CreateProjectRateAsResponseMessageAsync(input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(PortalMessages.UserNotFound);
    }

    [Fact]
    public async Task CreateRateWhenProjectDoesNotExistResponseWithNotFoundResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var input = new CreateProjectRateInput(1, 100000, 1, CurrencyCodes.USDollar, new DateOnly(2025, 1, 1));

        // Act
        var response = await client.CreateProjectRateAsResponseMessageAsync(input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(PortalMessages.ProjectNotFound);
    }

    [Fact]
    public async Task CreateRateWhenUserNotassignedToProjectResponseWith400()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        (var user, var project) = await GetUserAndProjectAsync(client);

        ExecuteMigrationTasks();

        var input = new CreateProjectRateInput(user.Id, project.Id, 1, CurrencyCodes.USDollar, new DateOnly(2025, 1, 1));

        // Act
        var response = await client.CreateProjectRateAsResponseMessageAsync(input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(PortalMessages.UserDoesNotHaveAccessToSomeProjects);
    }

    [Fact]
    public async Task CreateRateWhenRequestValidResponseWithRateCoreDto()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        (var user, var project) = await GetUserAndProjectAsync(client);

        var userProjectAssignment = await client.CreateProjectAssignment(user!.Id, project!.Id, new DateOnly(2025, 3, 1));

        ExecuteMigrationTasks();

        var input = new CreateProjectRateInput(user.Id, project.Id, 1, CurrencyCodes.USDollar, new DateOnly(2025, 3, 1));

        // Act
        var response = await client.CreateProjectRateAsResponseMessageAsync(input);

        // Assert
        var rate = await response.Content.ReadFromJsonAsync<ProjectRateCoreDto>(_jsonOptions, _cancellationToken);
        rate.Should().NotBeNull();
        rate.UserId.Should().Be(user.Id);
        rate.ProjectId.Should().Be(project.Id);
        rate.HourlyRate.Should().Be(input.HourlyRate);
        rate.CurrencyId.Should().Be(input.CurrencyId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateRateWhenDateConflictResponseWithConflict()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        (var user, var project) = await GetUserAndProjectAsync(client);

        var userProjectAssignment = await client.CreateProjectAssignment(user!.Id, project!.Id, new DateOnly(2025, 2, 1));

        ExecuteMigrationTasks();

        var input = new CreateProjectRateInput(user.Id, project.Id, 1, CurrencyCodes.USDollar, new DateOnly(2025, 2, 1));
        var secondInput = new CreateProjectRateInput(user.Id, project.Id, 10, CurrencyCodes.USDollar, new DateOnly(2025, 2, 1));

        await client.CreateProjectRateAsResponseMessageAsync(input);

        // Act
        var response = await client.CreateProjectRateAsResponseMessageAsync(secondInput);

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(PortalMessages.ProjectRateDateConflicts);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task EditRateWhenRateDoesNotExistResponseWithNotFoundResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var input = new EditProjectRateInput(1, CurrencyCodes.USDollar, new DateOnly(2025, 1, 1));

        // Act
        var response = await client.EditProjectRateAsResponseMessageAsync(id: 10000, input: input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(PortalMessages.ProjectRateNotFound);
    }

    [Fact]
    public async Task EditRateWhenRequestValidResponseWithDto()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var exDate = new DateOnly(2025, 3, 1);
        (var user, var project) = await GetUserAndProjectAsync(client);
        var userProjectAssignment = await client.CreateProjectAssignment(user!.Id, project!.Id, exDate);

        ExecuteMigrationTasks();

        var rate = await client.CreateProjectRateAsync(user.Id, project.Id, 1, CurrencyCodes.USDollar, exDate);

        var input = new EditProjectRateInput(1, CurrencyCodes.USDollar, exDate.AddMonths(1));

        // Act
        var response = await client.EditProjectRateAsResponseMessageAsync(id: rate!.Id, input: input);

        // Assert
        var edited = await response.Content.ReadFromJsonAsync<ProjectRateCoreDto>(_jsonOptions, _cancellationToken);
        edited.Should().NotBeNull();
        edited.UserId.Should().Be(user.Id);
        edited.ProjectId.Should().Be(project.Id);
        edited.HourlyRate.Should().Be(input.HourlyRate);
        edited.CurrencyId.Should().Be(input.CurrencyId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetRateHistoryWhenRequestValidResponseWithProjectRateHistoryCoreDto()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        (var user, var project) = await GetUserAndProjectAsync(client);
        _ = await client.CreateProjectAssignment(user!.Id, project!.Id, new DateOnly(2025, 3, 1));

        ExecuteMigrationTasks();

        var oldRate = await client.CreateProjectRateAsync(user.Id, project.Id, 1, CurrencyCodes.USDollar, new DateOnly(2025, 3, 1));
        var newRate = await client.CreateProjectRateAsync(user.Id, project.Id, 10, CurrencyCodes.USDollar, new DateOnly(2025, 4, 1));

        var expRates = new List<ProjectRateShortOutput>();
        foreach(var rate in new List<ProjectRateCoreDto> { oldRate!, newRate! })
        {
            expRates.Add(new ProjectRateShortOutput
            {
                Id = rate.Id,
                CurrencyId = rate.CurrencyId,
                HourlyRate = rate.HourlyRate,
                RateFrom = rate.RateFrom,
                CanRateBeManagedByCurrentUser = true
            });
        }

        // Act
        var response = await client.GetRateHistoryAsResponseMessageAsync(project.Id, user.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var history = await response.Content.ReadFromJsonAsync<ProjectRateHistoryOutput>(_jsonOptions, _cancellationToken);
        history.Should().NotBeNull();
        history.UserId.Should().Be(user.Id);
        history.ProjectId.Should().Be(project.Id);
        history.ProjectRates.Should().HaveCount(2);
        history.ProjectRates[0].Should().BeEquivalentTo(expRates[0]);
        history.ProjectRates[1].Should().BeEquivalentTo(expRates[1]);
    }

    [Fact]
    public async Task GetAllRatesWhenCannotViewProjectRatesResponseWith403()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var response = await client.GetRatesAsResponseMessageAsync(2026, 6, new GetProjectRatesInput());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAllRatesWhenWhenFilterBySingleDapartmentReturnFilteredRates()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var date = new DateOnly(2025, 3, 1);

        (var user1, var project) = await GetUserAndProjectAsync(client);
        _ = await client.CreateProjectAssignment(user1!.Id, project!.Id, date);
        var user2 = await client.CreateUser(GetCreateUserInput(departmentId: "BA", useResourceGroupId: false));
        _ = await client.CreateProjectAssignment(user2!.Id, project!.Id, date);

        ExecuteMigrationTasks();

        var rate1 = await client.CreateProjectRateAsync(user1.Id, project.Id, 1, CurrencyCodes.USDollar, date);

        var rate2 = await client.CreateProjectRateAsync(user2.Id, project.Id, 1, CurrencyCodes.USDollar, date);

        // Act
        var response = await client.GetRatesAsResponseMessageAsync(date.Year, date.Month,
            new GetProjectRatesInput { DepartmentIds = [user1.DepartmentId!] });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var rates = await response.Content.ReadFromJsonAsync<PageableListOfItems<UserProjectRatesOutput>>(_jsonOptions, _cancellationToken);
        rates!.Items.Should().OnlyContain(x => x.User.DepartmentId == user1.DepartmentId);
    }

    [Fact]
    public async Task GetAllRatesWhenWhenFilterByUserIdReturnFilteredRates()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var date = new DateOnly(2025, 3, 1);

        (var user, var project) = await GetUserAndProjectAsync(client);
        _ = await client.CreateProjectAssignment(user!.Id, project!.Id, date);

        ExecuteMigrationTasks();

        var rate = await client.CreateProjectRateAsync(user.Id, project.Id, 1, CurrencyCodes.USDollar, date);

        // Act
        var response = await client.GetRatesAsResponseMessageAsync(date.Year, date.Month,
            new GetProjectRatesInput { UserId = user.Id });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var rates = await response.Content.ReadFromJsonAsync<PageableListOfItems<UserProjectRatesOutput>>(_jsonOptions, _cancellationToken);
        rates!.Items.Single().User.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task GetAllRatesWhenWhenFilterByUserNameReturnFilteredRates()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var date = new DateOnly(2025, 3, 1);

        (var user, var project) = await GetUserAndProjectAsync(client, userLastName: "GetAllRatesWhenWhenFilterByUserNameReturnFilteredRates");
        _ = await client.CreateProjectAssignment(user!.Id, project!.Id, date);

        ExecuteMigrationTasks();

        var rate = await client.CreateProjectRateAsync(user.Id, project.Id, 1, CurrencyCodes.USDollar, date);

        // Act
        var response = await client.GetRatesAsResponseMessageAsync(date.Year, date.Month,
            new GetProjectRatesInput { UserFullName = user.LastName });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var rates = await response.Content.ReadFromJsonAsync<PageableListOfItems<UserProjectRatesOutput>>(_jsonOptions, _cancellationToken);
        rates!.Items.Single().User.LastName.Should().Be(user.LastName);
    }

    [Fact]
    public async Task GetAllRatesWhenWhenFilterByProjectNameReturnFilteredRates()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var date = new DateOnly(2025, 3, 1);

        (var user1, var project1) = await GetUserAndProjectAsync(client);
        _ = await client.CreateProjectAssignment(user1!.Id, project1!.Id, date);
        (var user2, var project2) = await GetUserAndProjectAsync(client);
        _ = await client.CreateProjectAssignment(user2!.Id, project2!.Id, date);

        ExecuteMigrationTasks();

        var rate1 = await client.CreateProjectRateAsync(user1.Id, project1.Id, 1, CurrencyCodes.USDollar, date);
        var rate2 = await client.CreateProjectRateAsync(user2.Id, project2.Id, 1, CurrencyCodes.USDollar, date);

        // Act
        var response = await client.GetRatesAsResponseMessageAsync(date.Year, date.Month,
            new GetProjectRatesInput { ProjectName = project1.Name });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var rates = await response.Content.ReadFromJsonAsync<PageableListOfItems<UserProjectRatesOutput>>(_jsonOptions, _cancellationToken);
        rates!.Items.Should().OnlyContain(x => x.ProjectRates.All(rate => rate.ProjectName == project1.Name));
    }

    [Fact]
    public async Task GetAllRatesWhenWhenFilterByClientNameReturnFilteredRates()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var date = new DateOnly(2025, 3, 1);
        var projClient = await client.CreateClientAsync(name: "GetAllRatesWhenWhenFilterByClientNameReturnFilteredRates clientName");

        (var user1, var project1) = await GetUserAndProjectAsync(client, clientId: projClient!.Id);
        _ = await client.CreateProjectAssignment(user1!.Id, project1!.Id, date);
        (var user2, var project2) = await GetUserAndProjectAsync(client);
        _ = await client.CreateProjectAssignment(user2!.Id, project2!.Id, date);

        ExecuteMigrationTasks();

        var rate1 = await client.CreateProjectRateAsync(user1.Id, project1.Id, 1, CurrencyCodes.USDollar, date);
        var rate2 = await client.CreateProjectRateAsync(user2.Id, project2.Id, 1, CurrencyCodes.USDollar, date);

        // Act
        var response = await client.GetRatesAsResponseMessageAsync(date.Year, date.Month,
            new GetProjectRatesInput { ClientName = projClient.Name });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var rates = await response.Content.ReadFromJsonAsync<PageableListOfItems<UserProjectRatesOutput>>(_jsonOptions, _cancellationToken);
        rates!.Items.Should().OnlyContain(x => x.ProjectRates.All(rate => rate.ClientName == projClient.Name));
    }


    [Fact]
    public async Task GetAllRatesWhenWhenFilterByClientLegalEntityReturnFilteredRates()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var date = new DateOnly(2025, 3, 1);

        var projClient = await client.CreateClientAsync(name: "GetAllRatesWhenWhenFilterByClientLegalEntityReturnFilteredRates name",
            legalEntityName: "GetAllRatesWhenWhenFilterByClientLegalEntityReturnFilteredRates clientName");

        (var user1, var project1) = await GetUserAndProjectAsync(client, clientId: projClient!.Id);
        _ = await client.CreateProjectAssignment(user1!.Id, project1!.Id, date);
        (var user2, var project2) = await GetUserAndProjectAsync(client);
        _ = await client.CreateProjectAssignment(user2!.Id, project2!.Id, date);

        ExecuteMigrationTasks();

        var rate1 = await client.CreateProjectRateAsync(user1.Id, project1.Id, 1, CurrencyCodes.USDollar, date);
        var rate2 = await client.CreateProjectRateAsync(user2.Id, project2.Id, 1, CurrencyCodes.USDollar, date);

        // Act
        var response = await client.GetRatesAsResponseMessageAsync(date.Year, date.Month,
            new GetProjectRatesInput { ClientLegalEntity = projClient.LegalEntityName });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var rates = await response.Content.ReadFromJsonAsync<PageableListOfItems<UserProjectRatesOutput>>(_jsonOptions, _cancellationToken);
        rates!.Items.Should().OnlyContain(x => x.ProjectRates.All(rate => rate.ClientLegalEntity == projClient.LegalEntityName));
    }

    private async Task<(Portal.Identity.Controllers.Contracts.Output.UserOutput? User,
        TimeTrack.Controllers.Contracts.Output.ProjectOutput? Project)> GetUserAndProjectAsync(
        HttpClient client, string? userLastName = null, int? clientId = null)
    {
        var user = await client.CreateUser(GetCreateUserInput(lastName: userLastName));
        var project = await CreateProjectOutputAsync(client, clientId: clientId);

        return (user, project);
    }

    private void ExecuteMigrationTasks()
    {
        using var scope = _factory.Services.CreateScope();
        var migrationTasks = scope.ServiceProvider.GetServices<IMigrationTask>();
        foreach (var migrationTask in migrationTasks)
        {
            migrationTask.Execute();
        }
    }

    private CreateUserInput GetCreateUserInput(string departmentId = "Net", bool useResourceGroupId = true, string? lastName = null)
    {
        return new CreateUserInput
        {
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = lastName ?? Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = departmentId,
            ResourceGroupId = useResourceGroupId ? _factory.ResourceGroupId : null,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1,
            ContractDateFrom = DateOnly.FromDateTime(DateTime.UtcNow),
            ContractDateTo = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(1),
        };
    }

    private async Task<TimeTrack.Controllers.Contracts.Output.ProjectOutput?> CreateProjectOutputAsync(
        HttpClient client, string? name = null, int? clientId = null)
    {
        return await client.CreateProject(
            clientId: clientId ?? _factory.ModsenClientId!.Value,
            isPublic: false,
            name: name ?? Guid.CreateVersion7().ToAlphabetic(),
            isBillable: true);
    }
}
