using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;
namespace Modsen.Portal.IntegrationTests.TeamBoard.EmployeePayload;

[Collection("GeneralCollection")]
public class EmployeePayloadIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public EmployeePayloadIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        SetupWorkingHours();
    }

    [Fact]
    public async Task GetEmployeesPayloadWhenAuthAsGroupManagerResponseWithNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var year = DateTime.UtcNow.Year;
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
            IsBillable = true,
            IsPublic = false
        };

        var createdProject = await HttpClientTeamboardProjectsExtensions.CreateProjectAsHttpResponse(client, projectToCreate)
            .WithResponseType<ProjectOutput>();

        var projectUserToCreate = new CreateUserProjectAssignmentInput(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            DateOnly.FromDateTime(DateTime.UtcNow), IsTimeOnProjectBillable: true, Fte: 1m, UserOnProjectSeniorityLevelId: "SeniorL2");

        var projectUserResponse = await client.CreateProjectAssignment(user!.Id, createdProject.Id,
            projectUserToCreate);

        projectUserResponse.IsSuccessStatusCode.Should().BeTrue();

        var input = new UpsertFactDto
        {
            ProjectId = createdProject.Id,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = Guid.CreateVersion7().ToAlphabetic(),
            FactTrackedInMinutes = 240
        };

        await client.AuthAs(user.Email);

        await client.CreateFactTimesheetAsHttpMessageAsync(input);

        await client.AuthAsGroupManager();

        // Act
        var response = await client.CreateEmployeePayloadCalendar(year,
            new GetEmployeePayloadInput())
            .WithResponseType<PayloadCoreDto>();

        // Assert
        response.Users.Should().NotContain(u => u.UserId == user.Id);
    }

    [Fact]
    public async Task GetEmployeesPayloadWhenAuthAsGroupManagerResponseWithProjectAndUser()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var year = DateTime.UtcNow.Year;
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
            ResourceGroupId = 1,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,

        };

        var user = await client.CreateUser(userToCreate);

        var contract = new CreateUserContractInput(
            user.Id,
            new DateOnly(year, 01, 01),
            new DateOnly(year, 12, 31),
            null,
            null,
            null,
            null
            );

        await client.CreateContracts(contract);

        await client.UpdateEmploymentHistory(new UpdateEmploymentHistoryInput(user.Id, new DateOnly(year, 10, 10), null));

        var projectToCreate = new CreateProjectInput
        {
            ClientId = _factory.ModsenClientId!.Value,
            Color = "#ffffff",
            ManagerId = null,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            IsBillable = false,
            IsPublic = false
        };

        var createdProjectResponse = await HttpClientTeamboardProjectsExtensions.CreateProjectAsHttpResponse(client, projectToCreate);

        var createdProject = await createdProjectResponse.Content.ReadFromJsonAsync<ProjectOutput>(_cancellationToken);

        var projectUserToCreate = new CreateUserProjectAssignmentInput(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            DateOnly.FromDateTime(DateTime.UtcNow), IsTimeOnProjectBillable: true, Fte: 1m, UserOnProjectSeniorityLevelId: "SeniorL2");

        var projectUserResponse = await client.CreateProjectAssignment(user.Id, createdProject.Id,
            projectUserToCreate);

        projectUserResponse.IsSuccessStatusCode.Should().BeTrue();

        var input = new UpsertFactDto
        {
            ProjectId = createdProject.Id,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = Guid.CreateVersion7().ToAlphabetic(),
            FactTrackedInMinutes = 240
        };

        await client.AuthAs(user.Email);

        await client.CreateTimesheetFact(input);

        await client.AuthAsGroupManager();

        // Act
        var response = await client.CreateEmployeePayloadCalendar(year,
            new GetEmployeePayloadInput())
            .WithResponseType<PayloadCoreDto>();

        // Assert
        response.Users.Should().Contain(u => u.UserId == user.Id);
        response.Projects.Should().Contain(p => p.ProjectId == createdProject.Id);
    }

    [Fact]
    public async Task UpdateUserPayloadFteAsyncWhenAuthAsGroupManagerResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var year = DateTime.UtcNow.Year;
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
            ResourceGroupId = 1,
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

        var createdProject = await HttpClientTeamboardProjectsExtensions.CreateProjectAsHttpResponse(client, projectToCreate)
            .WithResponseType<ProjectOutput>();

        var projectUserToCreate = new CreateUserProjectAssignmentInput(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            DateOnly.FromDateTime(DateTime.UtcNow), IsTimeOnProjectBillable: true, Fte: 1m, UserOnProjectSeniorityLevelId: "SeniorL2");

        var projectUserResponse = await client.CreateProjectAssignment(user!.Id, createdProject.Id,
            projectUserToCreate);

        projectUserResponse.IsSuccessStatusCode.Should().BeTrue();

        var input = new UpsertFactDto
        {
            ProjectId = createdProject.Id,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = Guid.CreateVersion7().ToAlphabetic(),
            FactTrackedInMinutes = 240
        };

        await client.AuthAs(user.Email);

        await client.CreateFactTimesheetAsHttpMessageAsync(input);

        await client.AuthAsGroupManager();

        // Act
        var payloadFteInput = new UpdateUserPayloadFteInput(1m);

        var response = await client.UpdateEmployeePayloadCalendar(user.Id, year, input.ReportForDate.Month,
            payloadFteInput);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateUserPayloadFteAsyncWhenAuthAsGroupManagerResponseWithForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var year = DateTime.UtcNow.Year;
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

        var createdProjectResponse = await HttpClientTeamboardProjectsExtensions.CreateProjectAsHttpResponse(client, projectToCreate);

        var createdProject = await createdProjectResponse.Content.ReadFromJsonAsync<ProjectOutput>(_cancellationToken);

        var projectUserToCreate = new CreateUserProjectAssignmentInput(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            DateOnly.FromDateTime(DateTime.UtcNow), IsTimeOnProjectBillable: true, Fte: 1m, UserOnProjectSeniorityLevelId: "SeniorL2");

        var projectUserResponse = await client.CreateProjectAssignment(user!.Id, createdProject.Id,
            projectUserToCreate);

        projectUserResponse.IsSuccessStatusCode.Should().BeTrue();

        var input = new UpsertFactDto
        {
            ProjectId = createdProject.Id,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = Guid.CreateVersion7().ToAlphabetic(),
            FactTrackedInMinutes = 240
        };

        await client.AuthAs(user.Email);

        await client.CreateFactTimesheetAsHttpMessageAsync(input);

        await client.AuthAsGroupManager();

        // Act
        var payloadFteInput = new UpdateUserPayloadFteInput(1m);

        var response = await client.UpdateEmployeePayloadCalendar(user.Id, year, input.ReportForDate.Month,
            payloadFteInput);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetEmployeesPayloadWhenFilterByResourceGroupNameResponseWithProjectAndUser()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var year = DateTime.UtcNow.Year;

        var groupId = await client.CreateResourceGroup(
            name: "Group number one",
            departmentId: "Net");
        var group = await client.GetResourceGroupAsync(groupId.Id);

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
            ResourceGroupId = group.Id,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
        };

        var user = await client.CreateUser(userToCreate);

        var contract = new CreateUserContractInput(
            user.Id,
            new DateOnly(year, 01, 01),
            new DateOnly(year, 12, 31),
            null,
            null,
            null,
            null
            );

        await client.CreateContracts(contract);

        await client.UpdateEmploymentHistory(new UpdateEmploymentHistoryInput(user.Id, new DateOnly(year, 10, 10), null));

        var projectToCreate = new CreateProjectInput
        {
            ClientId = _factory.ModsenClientId!.Value,
            Color = "#ffffff",
            ManagerId = null,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            IsBillable = false,
            IsPublic = false
        };

        var createdProjectResponse = await HttpClientTeamboardProjectsExtensions.CreateProjectAsHttpResponse(client, projectToCreate);

        var createdProject = await createdProjectResponse.Content.ReadFromJsonAsync<ProjectOutput>(_cancellationToken);

        var projectUserToCreate = new CreateUserProjectAssignmentInput(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            DateOnly.FromDateTime(DateTime.UtcNow), IsTimeOnProjectBillable: true, Fte: 1m, UserOnProjectSeniorityLevelId: "SeniorL2");

        var projectUserResponse = await client.CreateProjectAssignment(user.Id, createdProject.Id,
            projectUserToCreate);

        projectUserResponse.IsSuccessStatusCode.Should().BeTrue();

        var input = new UpsertFactDto
        {
            ProjectId = createdProject.Id,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = Guid.CreateVersion7().ToAlphabetic(),
            FactTrackedInMinutes = 240
        };

        await client.AuthAs(user.Email);

        await client.CreateTimesheetFact(input);

        await client.AuthAsCeo();

        // Act
        var response = await client.CreateEmployeePayloadCalendar(year,
            new GetEmployeePayloadInput() { ResourceGroupName = group.Name} )
            .WithResponseType<PayloadCoreDto>();

        // Assert
        response.Users.Should().Contain(u => u.UserId == user.Id);
        response.Projects.Should().Contain(p => p.ProjectId == createdProject.Id);
    }

    [Fact]
    public async Task GetEmployeesPayloadWhenFilterByProjectNameResponseWithProjectAndUser()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var year = DateTime.UtcNow.Year;
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
            ResourceGroupId = 1,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
        };

        var user = await client.CreateUser(userToCreate);

        var contract = new CreateUserContractInput(
            user.Id,
            new DateOnly(year, 01, 01),
            new DateOnly(year, 12, 31),
            null,
            null,
            null,
            null
            );

        await client.CreateContracts(contract);

        await client.UpdateEmploymentHistory(new UpdateEmploymentHistoryInput(user.Id, new DateOnly(year, 10, 10), null));

        var projectToCreate = new CreateProjectInput
        {
            ClientId = _factory.ModsenClientId!.Value,
            Color = "#ffffff",
            ManagerId = null,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            IsBillable = false,
            IsPublic = false
        };

        var createdProjectResponse = await HttpClientTeamboardProjectsExtensions.CreateProjectAsHttpResponse(client, projectToCreate);

        var createdProject = await createdProjectResponse.Content.ReadFromJsonAsync<ProjectOutput>(_cancellationToken);

        var projectUserToCreate = new CreateUserProjectAssignmentInput(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            DateOnly.FromDateTime(DateTime.UtcNow), IsTimeOnProjectBillable: true, Fte: 1m, UserOnProjectSeniorityLevelId: "SeniorL2");

        var projectUserResponse = await client.CreateProjectAssignment(user.Id, createdProject.Id,
            projectUserToCreate);

        projectUserResponse.IsSuccessStatusCode.Should().BeTrue();

        var input = new UpsertFactDto
        {
            ProjectId = createdProject.Id,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = Guid.CreateVersion7().ToAlphabetic(),
            FactTrackedInMinutes = 240
        };

        await client.AuthAs(user.Email);

        await client.CreateTimesheetFact(input);

        await client.AuthAsCeo();

        // Act
        var response = await client.CreateEmployeePayloadCalendar(year,
            new GetEmployeePayloadInput() { ProjectName = createdProject.Name })
            .WithResponseType<PayloadCoreDto>();

        // Assert
        response.Users.Should().Contain(u => u.UserId == user.Id);
        response.Projects.Should().Contain(p => p.ProjectId == createdProject.Id);
    }

    [Fact]
    public async Task GetEmployeesPayloadWhenFilterByUserFullNameResponseWithProjectAndUser()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var year = DateTime.UtcNow.Year;
        var userToCreate = new CreateUserInput
        {
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "ТестИмя",
            LastNameRu = "ТестФамилия",
            PatronymicRu = "Отчество",
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            Color = "#ffffff",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "Net",
            ResourceGroupId = 1,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
        };

        var user = await client.CreateUser(userToCreate);

        var contract = new CreateUserContractInput(
            user.Id,
            new DateOnly(year, 01, 01),
            new DateOnly(year, 12, 31),
            null,
            null,
            null,
            null
            );

        await client.CreateContracts(contract);

        await client.UpdateEmploymentHistory(new UpdateEmploymentHistoryInput(user.Id, new DateOnly(year, 10, 10), null));

        var projectToCreate = new CreateProjectInput
        {
            ClientId = _factory.ModsenClientId!.Value,
            Color = "#ffffff",
            ManagerId = null,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            IsBillable = false,
            IsPublic = false
        };

        var createdProjectResponse = await HttpClientTeamboardProjectsExtensions.CreateProjectAsHttpResponse(client, projectToCreate);

        var createdProject = await createdProjectResponse.Content.ReadFromJsonAsync<ProjectOutput>(_cancellationToken);

        var projectUserToCreate = new CreateUserProjectAssignmentInput(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            DateOnly.FromDateTime(DateTime.UtcNow), IsTimeOnProjectBillable: true, Fte: 1m, UserOnProjectSeniorityLevelId: "SeniorL2");

        var projectUserResponse = await client.CreateProjectAssignment(user.Id, createdProject.Id,
            projectUserToCreate);

        projectUserResponse.IsSuccessStatusCode.Should().BeTrue();

        var input = new UpsertFactDto
        {
            ProjectId = createdProject.Id,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = Guid.CreateVersion7().ToAlphabetic(),
            FactTrackedInMinutes = 240
        };

        await client.AuthAs(user.Email);

        await client.CreateTimesheetFact(input);

        await client.AuthAsCeo();

        // Act
        var response = await client.CreateEmployeePayloadCalendar(year,
            new GetEmployeePayloadInput() { UserFullName = user.FullName })
            .WithResponseType<PayloadCoreDto>();

        // Assert
        response.Users.Should().Contain(u => u.UserId == user.Id);
        response.Projects.Should().Contain(p => p.ProjectId == createdProject.Id);
        var projectDetails = response.Users.First(u => u.UserId == user.Id).Projects
            .First(p => p.ProjectId == createdProject.Id);
        projectDetails.IsActiveAssignement.Should().BeTrue();
        projectDetails.IsTimeOnProjectBillable.Should().BeTrue();
    }

    [Fact]
    public async Task GetEmployeesPayloadWhenUserHasNonActiveBillableAndActiveNonBillableAssignShouldBeValid()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var year = DateTime.UtcNow.Year;
        var userToCreate = new CreateUserInput
        {
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "ТестИмя",
            LastNameRu = "ТестФамилия",
            PatronymicRu = "Отчество",
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            Color = "#ffffff",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "Net",
            ResourceGroupId = 1,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
        };

        var user = await client.CreateUser(userToCreate);

        var contract = new CreateUserContractInput(
            user.Id,
            new DateOnly(year, 01, 01),
            new DateOnly(year, 12, 31),
            null,
            null,
            null,
            null
            );

        await client.CreateContracts(contract);

        await client.UpdateEmploymentHistory(new UpdateEmploymentHistoryInput(user.Id, new DateOnly(year, 10, 10), null));

        var nonBillableProjectToCreate = new CreateProjectInput
        {
            ClientId = _factory.ModsenClientId!.Value,
            Color = "#ffffff",
            ManagerId = null,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            IsBillable = false,
            IsPublic = false
        };

        var createdNonBillableProjectResponse = await HttpClientTeamboardProjectsExtensions.CreateProjectAsHttpResponse(client,
            nonBillableProjectToCreate);

        var createdNonBillableProject = await createdNonBillableProjectResponse.Content
            .ReadFromJsonAsync<ProjectOutput>(_cancellationToken);

        var activeProjectUserToCreate = new CreateUserProjectAssignmentInput(
            UserOnProjectFrom: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), UserOnProjectTo: null,
            IsTimeOnProjectBillable: false, Fte: 1m, UserOnProjectSeniorityLevelId: "SeniorL2");

        var createdActiveProjectUserResponse = await client.CreateProjectAssignment(user.Id, createdNonBillableProject!.Id,
            activeProjectUserToCreate);

        createdActiveProjectUserResponse.IsSuccessStatusCode.Should().BeTrue();

        var billableProjectToCreate = new CreateProjectInput
        {
            ClientId = _factory.ModsenClientId!.Value,
            Color = "#ffffff",
            ManagerId = null,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            IsBillable = true,
            IsPublic = false
        };

        var createdBillableProjectResponse = await HttpClientTeamboardProjectsExtensions.CreateProjectAsHttpResponse(client,
            billableProjectToCreate);

        var createdBillableProject = await createdBillableProjectResponse.Content
            .ReadFromJsonAsync<ProjectOutput>(_cancellationToken);

        var inactiveProjectUserToCreate = new CreateUserProjectAssignmentInput(
            UserOnProjectFrom: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-100)),
            UserOnProjectTo: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            IsTimeOnProjectBillable: true, Fte: 1m, UserOnProjectSeniorityLevelId: "SeniorL2");

        var createdInactiveProjectUserResponse = await client.CreateProjectAssignment(user.Id, createdBillableProject!.Id,
            inactiveProjectUserToCreate);

        createdInactiveProjectUserResponse.IsSuccessStatusCode.Should().BeTrue();

        var inputNonBillableProject = new UpsertFactDto
        {
            ProjectId = createdNonBillableProject.Id,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)),
            TaskName = Guid.CreateVersion7().ToAlphabetic(),
            FactTrackedInMinutes = 240
        };

        var inputBillableProject = new UpsertFactDto
        {
            ProjectId = createdBillableProject.Id,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7)),
            TaskName = Guid.CreateVersion7().ToAlphabetic(),
            FactTrackedInMinutes = 240
        };

        await client.AuthAs(user.Email);

        await client.CreateTimesheetFact(inputNonBillableProject);
        await client.CreateTimesheetFact(inputBillableProject);

        await client.AuthAsCeo();

        // Act
        var response = await client.CreateEmployeePayloadCalendar(year,
            new GetEmployeePayloadInput() { UserFullName = user.FullName })
            .WithResponseType<PayloadCoreDto>();

        // Assert
        response.Users.Should().Contain(u => u.UserId == user.Id);
        response.Projects.Should().Contain(p => p.ProjectId == createdNonBillableProject.Id);

        var billableProjectDetails = response.Users.First(x => x.UserId == user.Id)!.Projects
            .First(x => x.ProjectId == createdBillableProject.Id)!;
        billableProjectDetails.IsTimeOnProjectBillable.Should().BeTrue();
        billableProjectDetails.IsActiveAssignement.Should().BeFalse();

        var nonBillableProjectDetails = response.Users.First(x => x.UserId == user.Id)!.Projects
            .First(x => x.ProjectId == createdNonBillableProject.Id)!;
        nonBillableProjectDetails.IsTimeOnProjectBillable.Should().BeFalse();
        nonBillableProjectDetails.IsActiveAssignement.Should().BeTrue();
    }


    [Theory]
    [InlineData(true, false, true, false, true, false)]
    [InlineData(false, true, false, true, true, false)]
    [InlineData(false, false, false, false, true, false)]
    [InlineData(false, true, true, true, true, false)]
    [InlineData(false, false, true, false, true, false)]
    [InlineData(null, null, true, true, false, true)]
    [InlineData(null, true, null, true, true, false)]
    [InlineData(true, null, null, true, false, false)]
    [InlineData(false, null, true, true, false, true)]
    public async Task GetEmployeesPayloadWhenUserHasMultipleAssignsToSingleProjectShouldReturnValid(
        bool? isPastBillable, bool? isCurrentBillable, bool? isFutureBillable,
        bool expectedBillability, bool expectedActivity, bool expectedFutureActivity)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var year = DateTime.UtcNow.Year;
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
            ResourceGroupId = 1,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
            ContractDateFrom = new DateOnly(year - 2, 01, 01),
            ContractDateTo = new DateOnly(year, 12, 31),
        };

        var user = await client.CreateUser(userToCreate);

        await client.UpdateEmploymentHistory(new UpdateEmploymentHistoryInput(user.Id, new DateOnly(year - 2, 01, 01), null));

        // Project
        var projectToCreate = new CreateProjectInput
        {
            ClientId = _factory.ModsenClientId!.Value,
            Color = "#ffffff",
            ManagerId = null,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            IsBillable = false,
            IsPublic = false
        };

        var createdProjectResponse = await HttpClientTeamboardProjectsExtensions.CreateProjectAsHttpResponse(client,
            projectToCreate);

        var createdProject = await createdProjectResponse.Content
            .ReadFromJsonAsync<ProjectOutput>(_cancellationToken);

        // Past Project User Assignment
        if (isPastBillable.HasValue)
        {
            var pastProjectUserToCreate = new CreateUserProjectAssignmentInput(
                UserOnProjectFrom: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-100)),
                UserOnProjectTo: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2)),
                IsTimeOnProjectBillable: isPastBillable.Value, Fte: 1m, UserOnProjectSeniorityLevelId: "SeniorL2");

            var createdPastProjectUserResponse = await client.CreateProjectAssignment(user.Id, createdProject!.Id,
                pastProjectUserToCreate);

            createdPastProjectUserResponse.IsSuccessStatusCode.Should().BeTrue();
        }

        // Current Project User Assignment
        if (isCurrentBillable.HasValue)
        {
            var currentProjectUserToCreate = new CreateUserProjectAssignmentInput(
                UserOnProjectFrom: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                UserOnProjectTo: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                IsTimeOnProjectBillable: isCurrentBillable.Value, Fte: 1m, UserOnProjectSeniorityLevelId: "SeniorL2");

            var createdCurrentProjectUserResponse = await client.CreateProjectAssignment(user.Id, createdProject!.Id,
                currentProjectUserToCreate);

            createdCurrentProjectUserResponse.IsSuccessStatusCode.Should().BeTrue();
        }

        // Future Project User Assignment
        if (isFutureBillable.HasValue)
        {
            var futureProjectUserToCreate = new CreateUserProjectAssignmentInput(
                UserOnProjectFrom: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
                UserOnProjectTo: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(102)),
                IsTimeOnProjectBillable: isFutureBillable.Value, Fte: 1m, UserOnProjectSeniorityLevelId: "SeniorL2");

            var createdFutureProjectUserResponse = await client.CreateProjectAssignment(user.Id, createdProject!.Id,
                futureProjectUserToCreate);

            createdFutureProjectUserResponse.IsSuccessStatusCode.Should().BeTrue();
        }

        var expected = new PayloadAssignedProject()
        {
            ProjectId = createdProject!.Id,
            IsTimeOnProjectBillable = expectedBillability,
            IsActiveAssignement = expectedActivity,
            IsFutureActiveAssignment = expectedFutureActivity
        };

        // Act
        var response = await client.CreateEmployeePayloadCalendar(year,
            new GetEmployeePayloadInput() { UserFullName = user.FullName })
            .WithResponseType<PayloadCoreDto>();

        // Assert
        response.Users.Should().Contain(u => u.UserId == user.Id);
        response.Projects.Should().Contain(p => p.ProjectId == createdProject!.Id);

        var projectDetails = response.Users.First(x => x.UserId == user.Id).Projects;
        projectDetails.Should().HaveCount(1);
        projectDetails.Should().Contain(x => x.ProjectId == createdProject!.Id);

        var project = projectDetails.First(x => x.ProjectId == createdProject!.Id);
        project.Should().BeEquivalentTo(expected);
    }

    private void SetupWorkingHours()
    {
        using var scope = _factory.Services.CreateScope();

        var teamBoardDbContext = scope.ServiceProvider.GetRequiredService<TeamBoardDbContext>();

        teamBoardDbContext.WorkingHoursPerMonths.RemoveRange(teamBoardDbContext.WorkingHoursPerMonths);
        var year = DateTime.UtcNow.Year;

        for (var i = 1; i <= 12; i++)
        {
            teamBoardDbContext.WorkingHoursPerMonths.Add(new WorkingHoursPerMonth
            {
                Year = year,
                CountryId = 1,
                Hours = 1,
                MonthOfYear = i,
            });

        }

        teamBoardDbContext.SaveChanges();
    }
}
