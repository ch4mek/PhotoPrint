using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Bogus;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests.RequestDto;
namespace Modsen.Portal.IntegrationTests.TeamBoard.UserSpecialization;

[Collection("GeneralCollection")]
public class UserSpecializationIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly Faker<CreateUserInput> _createUserInputFaker = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public UserSpecializationIntegrationTests(IntegrationTestFactory factory)
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
    public async Task CreateUserSpecializationWhenRequestIsValidReturnNewSpecialization()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = _createUserInputFaker.Generate();

        var createdUser = await client.CreateUser(userToCreate);

        var departmentIds = new List<string> { };
        var hardSkillIds = new List<int> { 1 };
        var hardSkillsSeniroties = new HardSkillSeniorityDto[]
        {
            new()
            {
                Id = 1,
                SeniorityLevelId = "Junior"
            }
        };

        var specializationToCreate = new CreateSpecializationTemplateInput(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds,
            hardSkillIds
        );

        var createdSpecializationTemplate = await client.CreateSpecializationTempplates(specializationToCreate)
            .WithResponseType<SpecializationTemplateOutput>();

        var userId = createdUser.Id;
        var specializationTemplateId = createdSpecializationTemplate.Id;

        var userSpecializationToCreate = new CreateUserSpecializationInput(
            userId,
            specializationTemplateId,
            "JuniorL2",
            hardSkillsSeniroties
        );

        // Act
        var response = await client.CreateUserSpecialization(userSpecializationToCreate)
            .WithResponseType<UserSpecializationCoreDto>();

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().NotBe(0);
        response.SpecializationTemplateId.Should().Be(specializationTemplateId);
        response.IsPrimary.Should().BeTrue();
    }

    [Fact]
    public async Task CreateUserSpecializationWhenPrimaryAlreadyExistsReturnNewAdditionalSpecialization()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var departmentIds = new List<string> { };
        var hardSkillIds = new List<int> { 1 };
        var hardSkillsSeniroties = new HardSkillSeniorityDto[]
        {
            new()
            {
                Id = 1,
                SeniorityLevelId = "Junior"
            }
        };

        var specializationToCreate = new CreateSpecializationTemplateInput(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds,
            hardSkillIds
        );

        var createdSpecializationTemplate = await client.CreateSpecializationTempplates(specializationToCreate)
            .WithResponseType<SpecializationTemplateOutput>();

        var secondSpecializationToCreate = new CreateSpecializationTemplateInput(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds,
            hardSkillIds
        );

        var secondSpecializationTemplate = await client.CreateSpecializationTempplates(secondSpecializationToCreate)
            .WithResponseType<SpecializationTemplateOutput>();

        var userId = _factory.HeadOfDepartmentUserId ?? 1;
        var specializationTemplateId = createdSpecializationTemplate.Id;

        var primarySpecializationToCreate = new CreateUserSpecializationInput(
            userId,
            specializationTemplateId,
            "JuniorL2",
            hardSkillsSeniroties
        );

        await client.CreateUserSpecialization(
                primarySpecializationToCreate)
            .WithResponseType<UserSpecializationCoreDto>();

        //Act
        var additionalSpecializationToCreate = new CreateUserSpecializationInput(
            userId,
            secondSpecializationTemplate.Id,
            "JuniorL2",
            hardSkillsSeniroties
        );

        var additionalResponse = await client.CreateUserSpecialization(
            additionalSpecializationToCreate);

        // Assert
        additionalResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var additionalSpecialization = await additionalResponse.Content
            .ReadFromJsonAsync<UserSpecializationCoreDto>(_cancellationToken);
        additionalSpecialization.Should().NotBeNull();
        additionalSpecialization.IsPrimary.Should().BeFalse();
    }

    [Fact]
    public async Task GetUserSpecializationsLookupWhenRequestIsValidResponseWithOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var userId = _factory.EmployeeUserId!.Value;

        var departmentIds = new List<string> { };
        var hardSkillIds = new List<int> { 1 };
        var hardSkillsSeniroties = new HardSkillSeniorityDto[]
        {
            new()
            {
                Id = 1,
                SeniorityLevelId = "Junior"
            }
        };

        var specializationToCreate = new CreateSpecializationTemplateInput(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds,
            hardSkillIds
        );

        var createdSpecializationTemplate = await client.CreateSpecializationTempplates(
            specializationToCreate)
            .WithResponseType<SpecializationTemplateOutput>();

        var specializationTemplateId = createdSpecializationTemplate.Id;

        var userSpecializationToCreate = new CreateUserSpecializationInput(
            userId,
            specializationTemplateId,
            "JuniorL2",
            hardSkillsSeniroties
        );

        await client.CreateUserSpecialization(
                userSpecializationToCreate)
            .WithResponseType<UserSpecializationCoreDto>();

        // Act
        var response = await client.GetUserSpecializationLookup(userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUserSpecializationsLookupWhenNameIsSpecifiedAndRequestIsValidResponseWithOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var userId = _factory.EmployeeUserId!.Value;

        var departmentIds = new List<string> { };
        var hardSkillIds = new List<int> { 1 };
        var hardSkillsSeniroties = new HardSkillSeniorityDto[]
        {
            new()
            {
                Id = 1,
                SeniorityLevelId = "Junior"
            }
        };

        var specialzationTemplateName = "SpecializationTemplateName";
        var specializationToCreate = new CreateSpecializationTemplateInput(
            Name: specialzationTemplateName+ Guid.CreateVersion7().ToAlphabetic(),
            departmentIds,
            hardSkillIds
        );

        var secondSpecializationToCreate = new CreateSpecializationTemplateInput(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds,
            hardSkillIds
        );

        var createdSpecializationTemplate = await client.CreateSpecializationTempplates(
            specializationToCreate)
            .WithResponseType<SpecializationTemplateOutput>();

        var secondSpecializationTemplate = await client.CreateSpecializationTempplates(
            secondSpecializationToCreate)
            .WithResponseType<SpecializationTemplateOutput>();

        var specializationTemplateId = createdSpecializationTemplate.Id;

        var userSpecializationToCreate = new CreateUserSpecializationInput(
            userId,
            specializationTemplateId,
            "JuniorL2",
            hardSkillsSeniroties
        );

        var secondUserSpecializationToCreate = new CreateUserSpecializationInput(
            userId,
            secondSpecializationTemplate.Id,
            "JuniorL2",
            hardSkillsSeniroties
        );

        await client.CreateUserSpecialization(userSpecializationToCreate)
            .WithResponseType<UserSpecializationCoreDto>();

        await client.CreateUserSpecialization(secondUserSpecializationToCreate)
            .WithResponseType<UserSpecializationCoreDto>();

        // Act
        var response = await client.GetUserSpecializationLookup(userId, specialzationTemplateName);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var userSpecializations = await response.Content.ReadFromJsonAsync<UserSpecializationTemplateWithHardSkillsDto[]>(_cancellationToken);
        userSpecializations.Should().NotBeNull();
        userSpecializations.Should().ContainSingle();
        userSpecializations[0].SpecializationTemplateName.Should().Contain(specialzationTemplateName);
    }

    [Fact]
    public async Task UpdatePrimarySpecializationWhenRequestIsValidResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = _createUserInputFaker.Generate();
        var createdUser = await client.CreateUser(userToCreate);

        var departmentIds = new List<string> { };
        var hardSkillIds = new List<int> { 1 };
        var hardSkillsSeniroties = new HardSkillSeniorityDto[]
        {
            new()
            {
                Id = 1,
                SeniorityLevelId = "Junior"
            }
        };

        var createFirstSpecializationTemplateInput = new CreateSpecializationTemplateInput(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds,
            hardSkillIds
        );

        var createdFirstSpecializationTemplate = await client.CreateSpecializationTempplates(
            createFirstSpecializationTemplateInput)
            .WithResponseType<SpecializationTemplateOutput>();

        var userId = createdUser.Id;
        var firstSpecializationTemplateId = createdFirstSpecializationTemplate.Id;

        var oldPrimaryUserSpecializationCreateInput = new CreateUserSpecializationInput(
            userId,
            firstSpecializationTemplateId,
            "JuniorL2",
            hardSkillsSeniroties
        );

        await client.CreateUserSpecialization(
                oldPrimaryUserSpecializationCreateInput)
            .WithResponseType<UserSpecializationCoreDto>();

        var createSecondSpecializationTemplateInput = new CreateSpecializationTemplateInput(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds,
            hardSkillIds
        );

        var createdSecondSpecializationTemplate = await client.CreateSpecializationTempplates(
            createSecondSpecializationTemplateInput)
            .WithResponseType<SpecializationTemplateOutput>();

        var newPrimaryUserSpecializationCreateInput = new CreateUserSpecializationInput(
            userId,
            createdSecondSpecializationTemplate.Id,
            "JuniorL2",
            hardSkillsSeniroties
        );

        var newPrimaryUserSpecialization = await client.CreateUserSpecialization(
            newPrimaryUserSpecializationCreateInput)
            .WithResponseType<UserSpecializationCoreDto>();

        var updateInput = new UpdatePrimaryUserSpecializationInput(
            userId,
            newPrimaryUserSpecialization.SpecializationTemplateId
        );

        // Act
        var response = await client.UpdateUserPrimarySpecialization(updateInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var output = await response.Content.ReadFromJsonAsync<UserSpecializationCoreDto>(_cancellationToken);

        output!.UserId.Should().Be(userId);
        output.SpecializationTemplateId.Should().Be(createdSecondSpecializationTemplate.Id);
        output.IsPrimary.Should().BeTrue();
    }

    [Fact]
    public async Task UpdatePrimarySpecializationWhenUpdatingUsersNonAssignedSpecializationResponseWithBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = _createUserInputFaker.Generate();
        var createdUser = await client.CreateUser(userToCreate);

        var departmentIds = new List<string> { };
        var hardSkillIds = new List<int> { 1 };
        var hardSkillsSeniroties = new HardSkillSeniorityDto[]
        {
            new()
            {
                Id = 1,
                SeniorityLevelId = "Junior"
            }
        };

        var specializationBeforeToCreate = new CreateSpecializationTemplateInput(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds,
            hardSkillIds
        );

        var createdSpecializationTemplateBefore = await client.CreateSpecializationTempplates(
            specializationBeforeToCreate)
            .WithResponseType<SpecializationTemplateOutput>();

        var userId = createdUser.Id;
        var specializationTemplateBeforeId = createdSpecializationTemplateBefore.Id;

        var userSpecializationToCreate = new CreateUserSpecializationInput(
            userId,
            specializationTemplateBeforeId,
            "JuniorL2",
            hardSkillsSeniroties
        );

        await client.CreateUserSpecialization(
                userSpecializationToCreate)
            .WithResponseType<UserSpecializationCoreDto>();

        var specializationAfterToCreate = new CreateSpecializationTemplateInput(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds,
            hardSkillIds
        );

        var createdSpecializationTemplateAfter = await client.CreateSpecializationTempplates(
            specializationAfterToCreate)
            .WithResponseType<SpecializationTemplateOutput>();

        var updateInput = new UpdatePrimaryUserSpecializationInput(
            userId,
            createdSpecializationTemplateAfter.Id
        );

        // Act
        var response = await client.UpdateUserPrimarySpecialization(updateInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteUserSpecializationWhenDelitingPrimarySpecializationResponseWithBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = _createUserInputFaker.Generate();
        var createdUser = await client.CreateUser(userToCreate);

        var departmentIds = new List<string> { };
        var hardSkillIds = new List<int> { 1 };
        var hardSkillsSeniroties = new HardSkillSeniorityDto[]
        {
            new()
            {
                Id = 1,
                SeniorityLevelId = "Junior"
            }
        };

        var specializationToCreate = new CreateSpecializationTemplateInput(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds,
            hardSkillIds
        );

        var createdSpecializationTemplate = await client.CreateSpecializationTempplates(specializationToCreate)
            .WithResponseType<SpecializationTemplateOutput>();

        var userId = createdUser.Id;
        var specializationTemplateId = createdSpecializationTemplate.Id;

        var userSpecializationToCreate = new CreateUserSpecializationInput(
            userId,
            specializationTemplateId,
            "JuniorL2",
            hardSkillsSeniroties
        );

        var createdUserSpecialization = await client.CreateUserSpecialization(userSpecializationToCreate)
            .WithResponseType<UserSpecializationCoreDto>();

        var specializaitons = await client.GetUserSpecializationLookup(userId)
            .WithResponseType<List<UserSpecializationLookupOutput>>();

        var specializaitonId = specializaitons.Last(x => x.Id == createdUserSpecialization.Id).Id;

        // Act
        var responseMessage = await client.DeleteUserSpecialization(specializaitonId);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteUserSpecializationWhenRequestIsValidReturnDeletedSpecializationId()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = _createUserInputFaker.Generate();
        var createdUser = await client.CreateUser(userToCreate);

        var departmentIds = new List<string> { };
        var hardSkillIds = new List<int> { 1 };
        var hardSkillsSeniroties = new HardSkillSeniorityDto[]
        {
            new()
            {
                Id = 1,
                SeniorityLevelId = "Junior"
            }
        };

        var specializationToCreate = new CreateSpecializationTemplateInput(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds,
            hardSkillIds
        );

        var secondSpecializationToCreate = new CreateSpecializationTemplateInput(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds,
            hardSkillIds
        );

        var createdSpecializationTemplate = await client.CreateSpecializationTempplates(specializationToCreate)
            .WithResponseType<SpecializationTemplateOutput>();

        var secondCreatedSpecializationTemplate = await client.CreateSpecializationTempplates(secondSpecializationToCreate)
            .WithResponseType<SpecializationTemplateOutput>();

        var userId = createdUser.Id;
        var userSpecializationToCreate = new CreateUserSpecializationInput(
            userId,
            createdSpecializationTemplate.Id,
            "JuniorL2",
            hardSkillsSeniroties
        );

        var secondUserSpecializationToCreate = new CreateUserSpecializationInput(
            userId,
            secondCreatedSpecializationTemplate.Id,
            "JuniorL2",
            hardSkillsSeniroties
        );

        await client.CreateUserSpecialization(userSpecializationToCreate)
            .WithResponseType<UserSpecializationCoreDto>();

        var createdAdditionalSpecialization = await client.CreateUserSpecialization(secondUserSpecializationToCreate)
            .WithResponseType<UserSpecializationCoreDto>();

        // Act
        var responseMessage = await client.DeleteUserSpecialization(createdAdditionalSpecialization.Id);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

        var output = await responseMessage.Content.ReadFromJsonAsync<int>(_cancellationToken);
        output!.Should().Be(createdAdditionalSpecialization.Id);
    }

    [Fact]
    public async Task UpdateUserSpecializationsWhenRequestIsValidReturnsUpdatedSnapshot()
    {
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = _createUserInputFaker.Generate();
        var createdUser = await client.CreateUser(userToCreate);

        var departmentIds = new List<string> { };
        var hardSkillIds = new List<int> { 1 };

        var specializationToCreate = new CreateSpecializationTemplateInput(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds,
            hardSkillIds
        );

        var createdSpecializationTemplate = await client.CreateSpecializationTempplates(
            specializationToCreate)
            .WithResponseType<SpecializationTemplateOutput>();

        var specializationTemplateId = createdSpecializationTemplate.Id;
        var hardSkills = new HardSkillSeniorityDto[] { new() { Id = 1, SeniorityLevelId = "Junior" } };
        var userSpecializationToCreate = new CreateUserSpecializationInput(
            createdUser.Id,
            specializationTemplateId,
            "JuniorL2",
            hardSkills
        );

        await client.CreateUserSpecialization(
                userSpecializationToCreate)
            .WithResponseType<UserSpecializationCoreDto>();

        var getResponse = await client.GetUserSkillsSnapshot(createdUser.Id, 1, 0);

        var snapshots = await getResponse.Content.ReadFromJsonAsync<PageableListOfItems<UserSpecializationTemplateWithSnapshotDto>>(_cancellationToken);

        var updateUserSpecializationsInput = new UpdateUserSpecializationsInput(
            snapshots.Items.First().SpecializationSnapshots.First().Id,
            createdUser.Id,
            [
                new CreateUserSpecializationDto
                {
                    SpecializationTemplateId = specializationTemplateId,
                    SpecializationSeniorityLevelId = "SeniorL2",
                    HardSkills = [new HardSkillSeniorityDto { Id = 1, SeniorityLevelId = "Junior" }]
                }
            ]);

        //Act
        var response = await client.UpdateUserSpecialization(
            updateUserSpecializationsInput);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var output = await response.Content.ReadFromJsonAsync<UserSnapshotCoreDto>(_cancellationToken);
        output.Should().NotBeNull();
        output.UserId.Should().Be(createdUser.Id);
        output.UserSpecializations.Should().ContainSingle();
    }

    [Fact]
    public async Task UpdateUserSpecializationsWhenSnapshotDoesNotExistResponseNotFound()
    {
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var userId = _factory.EmployeeUserId!.Value;

        var departmentIds = new List<string> { };
        var hardSkillIds = new List<int> { 1 };

        var specializationToCreate = new CreateSpecializationTemplateInput(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds,
            hardSkillIds
        );

        var createdSpecializationTemplate = await client.CreateSpecializationTempplates(
            specializationToCreate)
            .WithResponseType<SpecializationTemplateOutput>();

        var specializationTemplateId = createdSpecializationTemplate.Id;

        var hardSkills = new HardSkillSeniorityDto[] {
            new() { Id = 1, SeniorityLevelId = "Junior" } };
        var userSpecializationToCreate = new CreateUserSpecializationInput(
            userId,
            specializationTemplateId,
            "JuniorL2",
            hardSkills
        );

        await client.CreateUserSpecialization(
                userSpecializationToCreate)
            .WithResponseType<UserSpecializationCoreDto>();

        var getResponse = await client.GetUserSkillsSnapshot(userId, 1, 0);
        await getResponse.Content.ReadFromJsonAsync<PageableListOfItems<UserSpecializationTemplateWithSnapshotDto>>(_cancellationToken);

        var updateUserSpecializationsInput = new UpdateUserSpecializationsInput(
            -1,
            userId,
            [
                new CreateUserSpecializationDto
                {
                    SpecializationTemplateId = specializationTemplateId,
                    SpecializationSeniorityLevelId = "SeniorL2",
                    HardSkills = [new HardSkillSeniorityDto { Id = 1, SeniorityLevelId = "Senior" }]
                }
            ]);

        //Act
        var response = await client.UpdateUserSpecialization(updateUserSpecializationsInput);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUserSpecializationsWhenAreAssignedFalseResponseWithNotAssignedToUserSpecializations()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var userId = _factory.EmployeeUserId!.Value;

        var departmentIds = new List<string> { };
        var hardSkillIds = new List<int> { 1 };
        var hardSkillsSeniroties = new HardSkillSeniorityDto[]
        {
            new()
            {
                Id = 1,
                SeniorityLevelId = "Junior"
            }
        };

        var specializationToCreate = new CreateSpecializationTemplateInput(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds,
            hardSkillIds
        );

        var createdSpecializationTemplate = await client.CreateSpecializationTempplates(
            specializationToCreate)
            .WithResponseType<SpecializationTemplateOutput>();

        var specializationTemplateId = createdSpecializationTemplate.Id;

        var userSpecializationToCreate = new CreateUserSpecializationInput(
            userId,
            specializationTemplateId,
            "JuniorL2",
            hardSkillsSeniroties
        );

        var firstUserSpecialization = await client.CreateUserSpecialization(userSpecializationToCreate)
            .WithResponseType<UserSpecializationCoreDto>();

        // Act
        var response = await client.GetAssignedUserSpecializations(userId, false);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var specializations = await response.Content.ReadFromJsonAsync<SpecializationTemplateWithHardSkillsDto[]>(_cancellationToken);
        specializations.Should().NotBeNull();
        specializations.All(x => x.SpecializationTemplateId != firstUserSpecialization.SpecializationTemplateId).Should().BeTrue();
    }

    [Fact]
    public async Task GetUserSpecializationsWhenAreAssignedFalseResponseWithAssignedToUserSpecializations()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = _createUserInputFaker.Clone().Generate();
        var createdUser = await client.CreateUser(userToCreate);

        var departmentIds = new List<string> { };
        var hardSkillIds = new List<int> { 1 };
        var hardSkillsSeniroties = new HardSkillSeniorityDto[]
        {
            new()
            {
                Id = 1,
                SeniorityLevelId = "Junior"
            }
        };

        var specializationToCreate = new CreateSpecializationTemplateInput(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds,
            hardSkillIds
        );

        var createdSpecializationTemplate = await client.CreateSpecializationTempplates(
            specializationToCreate)
            .WithResponseType<SpecializationTemplateOutput>();

        var specializationTemplateId = createdSpecializationTemplate.Id;

        var userSpecializationToCreate = new CreateUserSpecializationInput(
            createdUser.Id,
            specializationTemplateId,
            "JuniorL2",
            hardSkillsSeniroties
        );

        var firstUserSpecialization = await client.CreateUserSpecialization(
            userSpecializationToCreate)
            .WithResponseType<UserSpecializationCoreDto>();

        // Act
        var response = await client.GetAssignedUserSpecializations(createdUser.Id, true);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var specializations = await response.Content.ReadFromJsonAsync<SpecializationTemplateWithHardSkillsDto[]>(_cancellationToken);
        specializations.Should().NotBeNull();
        specializations.Should().ContainSingle();
        specializations.FirstOrDefault(x => x.SpecializationTemplateId == firstUserSpecialization.SpecializationTemplateId).Should().NotBeNull();
    }
}
