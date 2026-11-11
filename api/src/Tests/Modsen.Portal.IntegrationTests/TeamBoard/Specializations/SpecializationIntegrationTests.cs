using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Bogus;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Identity.Controllers.Contracts.Output;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests.RequestDto;
namespace Modsen.Portal.IntegrationTests.TeamBoard.Specializations;

[Collection("GeneralCollection")]
public class SpecializationIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly Faker<CreateUserInput> _createUserInputFaker = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public SpecializationIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;

        _createUserInputFaker = _createUserInputFaker
            .CustomInstantiator(x => new CreateUserInput
            {
                FirstName = Guid.CreateVersion7().ToAlphabetic(),
                LastName = Guid.CreateVersion7().ToAlphabetic(),
                FirstNameRu = "Имя",
                LastNameRu = "Фамилия",
                PatronymicRu = "Отчество",
                Color = x.Internet.Color(),
                Email = x.Internet.Email(provider: "modsen-software.com", uniqueSuffix: Guid.CreateVersion7().ToAlphabetic()),
                Position = PortalRoleNames.Employee,
                Location = x.PickRandom<LocationType>(),
                DepartmentId = "Net",
                CountryId = 1,
                RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
            });
    }

    [Fact]
    public async Task CreateSpecializationTemplateWhenRequestIsValidReturnNewSpecialization()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var departmentIds = new List<string> { "Net" };
        var hardSkillIds = new List<int> { };

        var specializationToCreate = new CreateSpecializationTemplateInput(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds,
            hardSkillIds
        );

        // Act
        var response = await client.CreateSpecializationTemaplate(specializationToCreate)
            .WithResponseType<SpecializationTemplateOutput>();

        // Assert
        response.Should().NotBeNull();
        response.Name.Should().Be(specializationToCreate.Name);
    }

    [Fact]
    public async Task CreateSpecializationTemplateForDepartmentDifferentFromGroupManagersReturnConflict()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsGroupManager();

        var departmentIds = new List<string> { "Java" };
        var hardSkillIds = new List<int> { };

        var specializationToCreate = new CreateSpecializationTemplateInput(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds,
            hardSkillIds
        );

        // Act
        var response = await client.CreateSpecializationTemaplate(specializationToCreate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateSpecializationTemplateWhenHardSkillIsDeletedReturnNewSpecializationWithoutDeletedHardSkill()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createHardSkillInput = new CreateHardSkillInput("NewHardSkill", "CID");

        var createdHardSkill = await client.PostAsJsonAsync<CreateHardSkillInput, HardSkillOutput>(
            "api/hard-skills", createHardSkillInput, _cancellationToken);

        await client.DeleteAsync($"api/hard-skills/{createdHardSkill.Id}", _cancellationToken);

        var departmentIds = new List<string> { "Net" };
        var hardSkillIds = new List<int> { createdHardSkill.Id };

        var specializationToCreate = new CreateSpecializationTemplateInput(
            Name: Guid.CreateVersion7().ToAlphabetic(), departmentIds, hardSkillIds);

        // Act
        var createdSpecializationResponse = await client.PostAsJsonAsync<CreateSpecializationTemplateInput, SpecializationTemplateOutput>(
            "api/specialization-templates", specializationToCreate, _cancellationToken);

        // Assert
        createdSpecializationResponse.Should().NotBeNull();
        createdSpecializationResponse.Name.Should().Be(specializationToCreate.Name);

        var response = await client.GetFromJsonAsync<SpecializationTemplateWithDepartmentsAndHardSkillsOutput>(
            $"api/specialization-templates/{createdSpecializationResponse.Id}", _cancellationToken);

        response!.HardSkills.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateSpecializationTemplateWhenRequestIsValidReturnUpdatedSpecialization()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var departmentIds = new List<string> { "Net" };
        var hardSkillIds = new List<int> { };

        var specializationToCreate = new CreateSpecializationTemplateInput(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds,
            hardSkillIds
        );

        var specializationDepartmentsIds = new string[2] { "Java", "React" };
        var specializationHardSkills = new List<SpecializationTemplateHardSkillDto>
        {
            new() { Id = 1, Name = "Windows" },
            new() { Id = 2, Name = "Linux" }
        };

        var createdSpecialization = await client.PostAsJsonAsync<CreateSpecializationTemplateInput, SpecializationTemplateOutput>(
            "api/specialization-templates", specializationToCreate, _cancellationToken);

        var specializationToUpdate = new UpdateSpecializationTemplateInput(
            Id: createdSpecialization!.Id,
            Name: Guid.CreateVersion7().ToAlphabetic(),
            DepartmentIds: specializationDepartmentsIds,
            HardSkills: specializationHardSkills
        );

        // Act
        var response = await client.PutAsJsonAsync<UpdateSpecializationTemplateInput, SpecializationTemplateWithDepartmentsAndHardSkillsDto>(
            "api/specialization-templates", specializationToUpdate, _cancellationToken);

        // Assert
        response.Should().NotBeNull();

        response.Name.Should().NotBeEquivalentTo(createdSpecialization.Name);
        response.Name.Should().Be(specializationToUpdate.Name);

        response.DepartmentIds.Should().NotBeNull();
        response.DepartmentIds.Should().BeEquivalentTo(specializationDepartmentsIds);

        response.HardSkills.Should().NotBeNull();
        response.HardSkills.Should().BeEquivalentTo(specializationHardSkills);
    }

    [Fact]
    public async Task UpdateSpecializationTemplateWhenNothingChangedReturnOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var departmentIds = new List<string> { "Net" };
        var hardSkillIds = new List<int> { };

        var specializationToCreate = new CreateSpecializationTemplateInput(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds,
            hardSkillIds
        );

        var createdSpecialization = await client.PostAsJsonAsync<CreateSpecializationTemplateInput, SpecializationTemplateOutput>(
            "api/specialization-templates", specializationToCreate, _cancellationToken);

        var specializationToUpdate = new UpdateSpecializationTemplateInput(
            Id: createdSpecialization!.Id, specializationToCreate.Name, [.. departmentIds], []);

        // Act
        var response = await client.PutAsJsonAsync(
            "api/specialization-templates", specializationToUpdate, _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateSpecializationTemplateWhenHardSkillIsDeletedReturnNewSpecializationWithoutDeletedHardSkill()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createHardSkillInput = new CreateHardSkillInput("NewHardSkill", "CID");

        var createdHardSkill = await client.CreateHardSkills(createHardSkillInput)
            .WithResponseType<HardSkillOutput>();

        await client.DeleteHardSkills(createdHardSkill.Id);

        var departmentIds = new List<string> { "Net" };
        var hardSkillIds = new List<int> { };

        var specializationToCreate = new CreateSpecializationTemplateInput(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds,
            hardSkillIds
        );

        var specializationDepartmentsIds = new string[2] { "Java", "React" };
        var specializationHardSkills = new List<SpecializationTemplateHardSkillDto>
        {
            new() { Id = createdHardSkill.Id, Name = createdHardSkill.Name }
        };

        var createdSpecialization = await client.CreateSpecializationTemaplate(specializationToCreate)
            .WithResponseType<SpecializationTemplateOutput>();

        var specializationToUpdate = new UpdateSpecializationTemplateInput(
            Id: createdSpecialization.Id,
            Name: Guid.CreateVersion7().ToAlphabetic(),
            DepartmentIds: specializationDepartmentsIds,
            HardSkills: specializationHardSkills
        );

        // Act
        var response = await client.UpdateSpecializationTemaplate(specializationToUpdate)
            .WithResponseType<SpecializationTemplateWithDepartmentsAndHardSkillsDto>();

        // Assert
        response.Should().NotBeNull();

        response.Name.Should().NotBeEquivalentTo(createdSpecialization.Name);
        response.Name.Should().Be(specializationToUpdate.Name);

        response.DepartmentIds.Should().NotBeNull();
        response.DepartmentIds.Should().BeEquivalentTo(specializationDepartmentsIds);

        response.HardSkills.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateSpecializationTemplateWhenUserFromChangedDepartmentIsAssignedToSpecializationShouldReturnForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var departmentIds = new List<string> { "Net" };
        var hardSkillIds = new List<int> { 1 };
        var hardSkillsSeniroties = new HardSkillSeniorityDto[]
        {
            new()
            {
                Id = 1,
                SeniorityLevelId = "Junior"
            }
        };

        var specializationToCreate = new CreateSpecializationTemplateInput(Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds, hardSkillIds);

        var createdSpecialization = await client.CreateSpecializationTemaplate(specializationToCreate)
            .WithResponseType<SpecializationTemplateOutput>();

        var userToCreate = _createUserInputFaker.Generate();

        var createdUser = await client.CreateUser(userToCreate);

        var userSpecializationToCreate = new CreateUserSpecializationInput(
            createdUser.Id,
            createdSpecialization.Id,
            "JuniorL2",
             hardSkillsSeniroties);

        await client.CreateUserSpecialization(userSpecializationToCreate)
            .WithResponseType<UserSpecializationCoreDto>();

        var specializationToUpdate = new UpdateSpecializationTemplateInput(
            Id: createdSpecialization.Id, specializationToCreate.Name, [], []);

        // Act
        var response = await client.UpdateSpecializationTemaplate(specializationToUpdate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetSpecializationTemplatesWhenDepartmentQueryParamSpecifiedResponseExpectedOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var departmentIds = new List<string> { "Net" };
        var hardSkillIds = new List<int> { };

        var createSpecializationInput = new CreateSpecializationTemplateInput(Guid.CreateVersion7().ToAlphabetic(),
            departmentIds, hardSkillIds);

        var createdSpecialization = await client.CreateSpecializationTemaplate(createSpecializationInput)
            .WithResponseType<SpecializationTemplateOutput>();

        var createAnotherSpecializationInput = new CreateSpecializationTemplateInput(Guid.CreateVersion7().ToAlphabetic(),
            departmentIds, hardSkillIds);

        await client.CreateSpecializationTemaplate(createAnotherSpecializationInput)
            .WithResponseType<SpecializationTemplateOutput>();

        var department = "Js";

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TeamBoardDbContext>();

        var specializationTemplateDepartment = new SpecializationTemplateDepartment
        {
            DepartmentId = department,
            SpecializationTemplateId = createdSpecialization.Id,
            CreatedById = _factory.CeoUserId!.Value,
            CreatedAtUtc = DateTime.UtcNow.Date
        };

        await dbContext.SpecializationTemplateDepartments.AddAsync(specializationTemplateDepartment, _cancellationToken);

        await dbContext.SaveChangesAsync(_cancellationToken);

        var query = $"?departmentId={department}&top=100&skip=0";

        // Act
        var response = await client.GetSpecializationTemaplate(query)
            .WithResponseType<PageableListOfItems<OdpSpecializationTemplate>>();

        // Assert
        response.Items.Should().NotBeNull();
        response.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetSpecializationTemplatesWhenAuthorizedAsCtoResponseExpectedOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var departmentIds = new List<string> { "Net" };
        var hardSkillIds = new List<int> { };

        var createSpecializationInput = new CreateSpecializationTemplateInput(Guid.CreateVersion7().ToAlphabetic(),
            departmentIds, hardSkillIds);

        var createdSpecialization = await client.CreateSpecializationTemaplate(createSpecializationInput)
            .WithResponseType<SpecializationTemplateOutput>();

        var createAnotherSpecializationInput = new CreateSpecializationTemplateInput(Guid.CreateVersion7().ToAlphabetic(),
            departmentIds, hardSkillIds);

        await client.CreateSpecializationTemaplate(createAnotherSpecializationInput)
            .WithResponseType<SpecializationTemplateOutput>();

        var department = "Js";

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TeamBoardDbContext>();

        var specializationTemplateDepartment = new SpecializationTemplateDepartment
        {
            DepartmentId = department,
            SpecializationTemplateId = createdSpecialization.Id,
            CreatedById = _factory.CtoUserId!.Value,
            CreatedAtUtc = DateTime.UtcNow.Date
        };

        await dbContext.SpecializationTemplateDepartments.AddAsync(specializationTemplateDepartment, _cancellationToken);

        await dbContext.SaveChangesAsync(_cancellationToken);

        var query = $"?departmentId={department}&top=100&skip=0";

        var expectedSpecializationOutput = new OdpSpecializationTemplate
        {
            Id = createdSpecialization.Id,
            Name = createdSpecialization.Name,
            CanBeUpdated = true,
            CreatedById = _factory.CtoUserId!.Value
        };

        // Act
        var response = await client.GetSpecializationTemaplate(query)
            .WithResponseType<PageableListOfItems<OdpSpecializationTemplate>>();

        // Assert
        response?.Items.FirstOrDefault(x => x.Id == createdSpecialization.Id)
            .Should()
            .BeEquivalentTo(expectedSpecializationOutput);
    }

    [Fact]
    public async Task GetSpecializationTemplatesWhenAuthorizedAsEmployeeReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var response = await client.GetSpecializationTemaplate("");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetSpecializationTemplateByIdWhenAuthorizedAsEmployeeReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var id = 2;

        // Act
        var response = await client.GetSpecializationTemaplateById(id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetSpecializationTemplateByIdWhenSpecializationDoesNotExistReturnNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var id = 88;

        // Act
        var response = await client.GetSpecializationTemaplateById(id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetSpecializationByIdWhenSpecializationExistReturnSpecialization()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var departmentIds = new List<string> { "Net" };
        var hardSkillIds = new List<int> { };

        var specializationToCreate = new CreateSpecializationTemplateInput(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds,
            hardSkillIds
        );

        // Act
        var createdSpecialization = await client.CreateSpecializationTemaplate(specializationToCreate)
            .WithResponseType<SpecializationTemplateOutput>();

        var response = await client.GetSpecializationTemaplateById(createdSpecialization.Id)
            .WithResponseType<SpecializationTemplateWithDepartmentsAndHardSkillsOutput>();

        // Assert
        response.Should().NotBeNull();
        response.Name.Should().BeEquivalentTo(createdSpecialization.Name);
    }

    [Fact]
    public async Task DeleteSpecializationTemplateWhenRequestIsValidReturnDeletedSpecializationTemplateId()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var departmentIds = new List<string> { "Net" };
        var hardSkillIds = new List<int> { };

        var specializationToCreate = new CreateSpecializationTemplateInput(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds,
            hardSkillIds
        );

        var createdSpecialization = await client.PostAsJsonAsync<CreateSpecializationTemplateInput, SpecializationTemplateOutput>(
            "api/specialization-templates", specializationToCreate, _cancellationToken);

        // Act
        var response = await client.DeleteSpecializationTemaplate(createdSpecialization.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().NotBeNull();

        var output = await response.Content.ReadFromJsonAsync<DeleteSpecializationTemplateOutput>(_cancellationToken);

        output.Id.Should().Be(createdSpecialization.Id);
        output.Name.Should().BeEquivalentTo(specializationToCreate.Name);
    }

    [Fact]
    public async Task GetSpecializationTemplatesLookupWithAreAssignedIsFalseReturnSpecializationTemplatesUserDoesNotHave()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = _factory.EmployeeUserId!.Value;

        var user = await client.GetUserAsHttpMessage(userId)
            .WithResponseType<UserOutput>();

        var departmentIds = new List<string> { user.DepartmentId };
        var hardSkillIds = new List<int> { 1 };
        var hardSkillsSeniroties = new HardSkillSeniorityDto[]
        {
            new()
            {
                Id = 1,
                SeniorityLevelId = "Junior"
            }
        };

        var specializationsToCreate = new List<CreateSpecializationTemplateInput>
        {
            new(Name: Guid.CreateVersion7().ToAlphabetic(), departmentIds, hardSkillIds),
            new(Name: Guid.CreateVersion7().ToAlphabetic(), departmentIds, hardSkillIds),
            new(Name: Guid.CreateVersion7().ToAlphabetic(), departmentIds, hardSkillIds)
        };

        var specializations = new List<SpecializationTemplateLookupOutput>();

        foreach (var specializationToCreate in specializationsToCreate)
        {
            var createdSpecialization = await client.CreateSpecializationTemaplate(specializationToCreate)
                .WithResponseType<SpecializationTemplateOutput>();

            specializations.Add(new SpecializationTemplateLookupOutput(createdSpecialization.Id, createdSpecialization.Name));
        }

        var userSpecializationToCreate = new CreateUserSpecializationInput(
            userId,
            specializations.Last().Id,
            "JuniorL2",
            HardSkills: hardSkillsSeniroties);

        await client.CreateUserSpecialization(userSpecializationToCreate)
            .WithResponseType<UserSpecializationCoreDto>();

        var shouldReturn = specializations.GetRange(0, 2);

        var shouldNotReturn = specializations[2];

        // Act
        var response = await client.GetUserSpecializationTemaplate(userId, false);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var specializationsUserDoesNotHave = await response.Content
            .ReadFromJsonAsync<List<SpecializationTemplateLookupOutput>>(_jsonOptions,
                _cancellationToken);

        specializationsUserDoesNotHave.Should().Contain(shouldReturn);
        specializationsUserDoesNotHave.Should().NotContain(shouldNotReturn);
    }

    [Fact]
    public async Task GetSpecializationTemplatesLookupWithAreAssignedIsTrueReturnSpecializationTemplatesUserHas()
    {
         // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var departmentIds = new List<string> { "Net" };
        var hardSkillIds = new List<int> { 1 };
        var hardSkillsSeniroties = new HardSkillSeniorityDto[]
        {
            new()
            {
                Id = 1,
                SeniorityLevelId = "Junior"
            }
        };

        var specializationsToCreate = new List<CreateSpecializationTemplateInput>
        {
            new(Name: Guid.CreateVersion7().ToAlphabetic(), departmentIds, hardSkillIds),
            new(Name: Guid.CreateVersion7().ToAlphabetic(), departmentIds, hardSkillIds),
            new(Name: Guid.CreateVersion7().ToAlphabetic(), departmentIds, hardSkillIds)
        };

        var createdSpecializations = new List<SpecializationTemplateOutput>();

        foreach (var specializationToCreate in specializationsToCreate)
        {
            createdSpecializations.Add(await client.CreateSpecializationTemaplate(specializationToCreate)
                .WithResponseType<SpecializationTemplateOutput>());
        }

        var userId = _factory.EmployeeUserId!.Value;

        var userSpecializationToCreate = new CreateUserSpecializationInput(
            userId,
            createdSpecializations.Last().Id,
            "JuniorL2",
            HardSkills: hardSkillsSeniroties);

        await client.CreateUserSpecialization(userSpecializationToCreate)
            .WithResponseType<UserSpecializationCoreDto>();

        var shouldNotReturn = new List<SpecializationTemplateLookupOutput>
        {
            new(createdSpecializations[0].Id, createdSpecializations[0].Name),
            new(createdSpecializations[1].Id, createdSpecializations[1].Name)
        };

        var shouldReturn = new SpecializationTemplateLookupOutput(createdSpecializations[2].Id,
            createdSpecializations[2].Name);

        // Act
        var response = await client.GetUserSpecializationTemaplate(userId, true);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var specializationsUserHas = await response.Content
            .ReadFromJsonAsync<List<SpecializationTemplateLookupOutput>>(_jsonOptions,
                _cancellationToken);

        specializationsUserHas!.Should().Contain(shouldReturn);
        specializationsUserHas.IntersectBy(shouldNotReturn, x => x).Should().BeEmpty();
    }

    [Fact]
    public async Task GetSpecializationTemplatesLookupWithAreAssignedIsNullReturnSpecializationTemplatesUserHas()
    {
         // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var departmentIds = new List<string> { "Net" };
        var hardSkillIds = new List<int> { 1 };
        var hardSkillsSeniroties = new HardSkillSeniorityDto[]
        {
            new()
            {
                Id = 1,
                SeniorityLevelId = "Junior"
            }
        };

        var specializationsToCreate = new List<CreateSpecializationTemplateInput>
        {
            new(Name: Guid.CreateVersion7().ToAlphabetic(), departmentIds, hardSkillIds),
            new(Name: Guid.CreateVersion7().ToAlphabetic(), departmentIds, hardSkillIds),
            new(Name: Guid.CreateVersion7().ToAlphabetic(), departmentIds, hardSkillIds)
        };

        var createdSpecializations = new List<SpecializationTemplateLookupOutput>();

        foreach (var specializationToCreate in specializationsToCreate)
        {
            createdSpecializations.Add(await client.CreateSpecializationTemaplate(specializationToCreate)
                .WithResponseType<SpecializationTemplateLookupOutput>());
        }

        var userId = _factory.EmployeeUserId!.Value;

        var userSpecializationToCreate = new CreateUserSpecializationInput(
            userId,
            createdSpecializations.Last().Id,
            "JuniorL2",
            HardSkills: hardSkillsSeniroties);

        await client.CreateUserSpecialization(userSpecializationToCreate)
            .WithResponseType<UserSpecializationCoreDto>();

        var shouldNotReturn = new List<SpecializationTemplateLookupOutput>
        {
            new(createdSpecializations[0].Id, createdSpecializations[0].Name),
            new(createdSpecializations[1].Id, createdSpecializations[1].Name)
        };

        var shouldReturn = new SpecializationTemplateLookupOutput(createdSpecializations[2].Id,
            createdSpecializations[2].Name);

        // Act
        var response = await client.GetUserSpecializationTemaplate(userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var specializationsUserHas = await response.Content
            .ReadFromJsonAsync<List<SpecializationTemplateLookupOutput>>(_jsonOptions,
                _cancellationToken);

        specializationsUserHas!.Should().Contain(shouldReturn);
        specializationsUserHas.IntersectBy(shouldNotReturn, x => x).Should().BeEmpty();
    }

    [Fact]
    public async Task GetSpecializationTemplatesLookupWhenSpecializationDeletedShouldNotReturnDeletedSpecialization()
    {
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var departmentIds = new List<string> { "Net" };
        var hardSkillIds = new List<int> { };

        var specializationsToCreate = new List<CreateSpecializationTemplateInput>
        {
            new(Name: Guid.CreateVersion7().ToAlphabetic(), departmentIds, hardSkillIds),
            new(Name: Guid.CreateVersion7().ToAlphabetic(), departmentIds, hardSkillIds),
            new(Name: Guid.CreateVersion7().ToAlphabetic(), departmentIds, hardSkillIds)
        };

        var createdSpecializations = new List<SpecializationTemplateLookupOutput>();

        foreach (var specializationToCreate in specializationsToCreate)
        {
            createdSpecializations.Add(await client.CreateSpecializationTemaplate(specializationToCreate)
                .WithResponseType<SpecializationTemplateLookupOutput>());
        }

        var specializationIdToDeactivate = createdSpecializations[0].Id;
        await client.DeleteSpecializationTemaplate(specializationIdToDeactivate);

        var userId = _factory.EmployeeUserId!.Value;

        // Act
        var response = await client.GetUserSpecializationTemaplate(userId, false);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedSpecializationTemplates = await response.Content
            .ReadFromJsonAsync<List<SpecializationTemplateLookupOutput>>(_jsonOptions,
                _cancellationToken);

        returnedSpecializationTemplates.Should().NotContain(x => x.Id == specializationIdToDeactivate);
        returnedSpecializationTemplates.Should().Contain(x => x.Id == createdSpecializations[1].Id);
        returnedSpecializationTemplates.Should().Contain(x => x.Id == createdSpecializations[2].Id);
    }

    [Fact]
    public async Task UpdateSpecializationTemplateWhenLeavingAssignSpecializationWithoutHardSkillsReturnConflict()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var departmentIds = new List<string> { };
        var hardSkillIds = new List<int> { 1 };

        var specializationToCreate = new CreateSpecializationTemplateInput(Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds, hardSkillIds);

        var createdSpecialization = await client.CreateSpecializationTemaplate(specializationToCreate)
            .WithResponseType<SpecializationTemplateOutput>();

        var userToCreate = _createUserInputFaker.Generate();

        var createdUser = await client.CreateUser(userToCreate);

        var userSpecializationToCreate = new CreateUserSpecializationInput(
            createdUser.Id,
            createdSpecialization.Id,
            "JuniorL2",
            [new HardSkillSeniorityDto { Id = 1, SeniorityLevelId = "Junior" }]);

        await client.CreateUserSpecialization(userSpecializationToCreate)
            .WithResponseType<UserSpecializationCoreDto>();

        var specializationToUpdate = new UpdateSpecializationTemplateInput(
            Id: createdSpecialization.Id, specializationToCreate.Name, [], []);

        // Act
        var response = await client.UpdateSpecializationTemaplate(specializationToUpdate);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
