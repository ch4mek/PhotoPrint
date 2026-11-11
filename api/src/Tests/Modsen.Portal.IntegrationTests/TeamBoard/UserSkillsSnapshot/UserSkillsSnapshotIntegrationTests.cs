using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Bogus;
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
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests.RequestDto;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests.RequestDto;
namespace Modsen.Portal.IntegrationTests.TeamBoard.UserSkillsSnapshot;

[Collection("GeneralCollection")]
public class UserSkillsSnapshotIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly Faker<CreateUserInput> _createUserInputFaker = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private SpecializationTemplateOutput _createdSpecializationTemplate;

    public UserSkillsSnapshotIntegrationTests(IntegrationTestFactory factory)
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

    private async Task CreateSpecializationTemplateAsync()
    {
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var departmentIds = new List<string> { };
        var hardSkillIds = new List<int> { 1 };

        var specializationToCreate = new CreateSpecializationTemplateInput(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds,
            hardSkillIds
        );

        _createdSpecializationTemplate = await client.CreateSpecializationTemaplate(specializationToCreate)
            .WithResponseType<SpecializationTemplateOutput>();
    }

    private async Task CreateSpecializationTemplateAsync(List<int> hardSkillIds, List<string> departmentIds)
    {
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var specializationToCreate = new CreateSpecializationTemplateInput(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds,
            hardSkillIds
        );

        _createdSpecializationTemplate = await client.CreateSpecializationTemaplate(specializationToCreate)
            .WithResponseType<SpecializationTemplateOutput>();
    }

    [Fact]
    public async Task GetUserSnapshotsByUserIdWhenRequestIsValidReturnsEmptyList()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = _createUserInputFaker.Generate();

        var createdUser = await client.CreateUser(userToCreate);

        // Act
        var response = await client.GetUserSkillsSnapshot(createdUser.Id, 1, 0);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var output = await response.Content.ReadFromJsonAsync<PageableListOfItems<UserSpecializationTemplateWithSnapshotDto>>(_cancellationToken);
        output.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUserSnapshotsByUserIdWhenAuthorizedAsEmployeeReturnOk()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var response = await client.GetUserSkillsSnapshot(_factory.EmployeeUserId, 1, 0);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUserSnapshotsByUserIdWhenRequestIsValidReturnsSnapshots()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = _createUserInputFaker.Generate();

        var createdUser = await client.CreateUser(userToCreate);

        await CreateSpecializationTemplateAsync();

        var specializationTemplateId = _createdSpecializationTemplate.Id;
        var hardSkills = new HardSkillSeniorityDto[] {
            new() { Id = 1, SeniorityLevelId = "Junior" } };
        var userSpecializationToCreate = new CreateUserSpecializationInput(
            createdUser.Id,
            specializationTemplateId,
            "JuniorL2",
            hardSkills
        );

        var createdSpecializationResponse = await client
            .CreateUserSpecialization(userSpecializationToCreate)
            .WithResponseType<UserSpecializationCoreDto>();

        // Act
        var response = await client.GetUserSkillsSnapshot(createdUser.Id, 1, 0);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var snapshots = await response.Content.ReadFromJsonAsync<PageableListOfItems<UserSpecializationTemplateWithSnapshotDto>>(_cancellationToken);

        snapshots.Items.Should().NotBeNull();
        snapshots.Items.Should().ContainSingle();
        snapshots.Items.First().SpecializationSnapshots.First().UserSpecializations.First().SeniorityLevelId.Should()
            .Be(createdSpecializationResponse.SeniorityLevelId);
    }

    [Fact]
    public async Task GetUserSnapshotByUserIdWhenHardSkillRemovedFromSpecializationTemplateShouldBeInactive()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var hardSkills = new HardSkillSeniorityDto[]
        {
            new() { Id = 1, SeniorityLevelId = "Junior" },
            new() { Id = 2, SeniorityLevelId = null },
            new() { Id = 3, SeniorityLevelId = "Trainee" },
            new() { Id = 4, SeniorityLevelId = null }
        };

        var departmentIds = new List<string> { "Net" };

        await CreateSpecializationTemplateAsync(hardSkills.Select(x => x.Id).ToList(), departmentIds);

        var userToCreate = _createUserInputFaker.Generate();
        var createdUser = await client.CreateUser(userToCreate);

        var userSpecializationToCreate = new CreateUserSpecializationInput(createdUser!.Id,
            _createdSpecializationTemplate.Id, "JuniorL2", hardSkills);

        await client.CreateUserSpecialization(userSpecializationToCreate);

        var fetchedHardSkills = await client.GetHardSkillsLookup().WithResponseType<List<HardSkillLookupOutput>>();

        var hardSkillsToUpdate = fetchedHardSkills.Where(x => x.Id <= 2).Select(x => new SpecializationTemplateHardSkillDto
        {
            Id = x.Id,
            Name = x.Name
        }).ToList();

        var specializationTemplateToUpdate = new UpdateSpecializationTemplateInput(_createdSpecializationTemplate.Id,
            _createdSpecializationTemplate.Name, departmentIds, hardSkillsToUpdate);

        await client.UpdateSpecializationTemaplate(specializationTemplateToUpdate);

        // Act
        var response = await client.GetUserSkillsSnapshot(createdUser.Id, 1, 0);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var snapshots = await response.Content.ReadFromJsonAsync<PageableListOfItems<UserSpecializationTemplateWithSnapshotDto>>(_cancellationToken);

        snapshots.Total.Should().Be(1);
        snapshots.Items.Should().HaveCount(1);
        snapshots.Items.First().SpecializationSnapshots.First().UserId.Should().Be(createdUser.Id);
        snapshots.Items.First().SpecializationSnapshots.First().UserSpecializations
            .First().SpecializationTemplateId.Should().Be(_createdSpecializationTemplate.Id);

        var actualHardSkills = snapshots.Items.First().SpecializationSnapshots.First().UserSpecializations
            .First().HardSkills;
        actualHardSkills.Should().Contain(x => x.Id == 1 && x.SeniorityLevelId == "Junior" && x.IsActive.Value);
        actualHardSkills.Should().Contain(x => x.Id == 2 && x.SeniorityLevelId == null && x.IsActive.Value);
        actualHardSkills.Should().Contain(x => x.Id == 3 && x.SeniorityLevelId == "Trainee" && !x.IsActive.Value);
    }

    [Fact]
    public async Task GetUserSnapshotByDateWhenRequestIsValidReturnsSnapshot()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = _createUserInputFaker.Generate();

        var createdUser = await client.CreateUser(userToCreate);

        await CreateSpecializationTemplateAsync();

        var specializationTemplateId = _createdSpecializationTemplate.Id;
        var hardSkills = new HardSkillSeniorityDto[] {
            new() { Id = 1, SeniorityLevelId = "Junior" } };
        var userSpecializationToCreate = new CreateUserSpecializationInput(
            createdUser.Id,
            specializationTemplateId,
            "JuniorL2",
            hardSkills
        );

        await client
            .CreateUserSpecialization(userSpecializationToCreate)
            .WithResponseType<UserSpecializationCoreDto>();

        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var response = await client.GetUserSkillsSnapshotByDate(createdUser.Id, date);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var snapshot = await response.Content.ReadFromJsonAsync<UserSnapshotCoreDto>(_cancellationToken);

        snapshot.Should().NotBeNull();
        snapshot.SnapshotDate.Should().Be(date);
        snapshot.UserSpecializations.Should().ContainSingle();
        snapshot.UserSpecializations.First().SpecializationTemplateId.Should().Be(specializationTemplateId);
    }

    [Fact]
    public async Task GetUserSnapshotByDateWhenSnapshotDoesntExistReturnsNotFound()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = _createUserInputFaker.Generate();

        var createdUser = await client.CreateUser(userToCreate);

        await CreateSpecializationTemplateAsync();

        var specializationTemplateId = _createdSpecializationTemplate.Id;
        var hardSkills = new HardSkillSeniorityDto[] {
            new() { Id = 1, SeniorityLevelId = "Junior" } };
        var userSpecializationToCreate = new CreateUserSpecializationInput(
            createdUser.Id,
            specializationTemplateId,
            "JuniorL2",
            hardSkills
        );

        await client
            .CreateUserSpecialization(userSpecializationToCreate)
            .WithResponseType<UserSpecializationCoreDto>();

        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5));

        // Act
        var response = await client.GetUserSkillsSnapshotByDate(createdUser.Id, date);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUserSnapshotByIdWhenRequestIsValidReturnsSnapshot()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = _createUserInputFaker.Generate();

        var createdUser = await client.CreateUser(userToCreate);

        await CreateSpecializationTemplateAsync();

        var specializationTemplateId = _createdSpecializationTemplate.Id;
        var hardSkills = new HardSkillSeniorityDto[] {
            new() { Id = 1, SeniorityLevelId = "Junior" } };
        var userSpecializationToCreate = new CreateUserSpecializationInput(
            createdUser.Id,
            specializationTemplateId,
            "JuniorL2",
            hardSkills
        );

        await client
            .CreateUserSpecialization(userSpecializationToCreate)
            .WithResponseType<UserSpecializationCoreDto>();

        var getAllResponse = await client.
            GetUserSkillsSnapshot(createdUser.Id, 1, 0);

        var userSnapshots = await getAllResponse.Content.ReadFromJsonAsync<PageableListOfItems<UserSpecializationTemplateWithSnapshotDto>>(_cancellationToken);
        var snapshotId = userSnapshots.Items.First().SpecializationSnapshots.Last().Id;
        // Act
        var response = await client.GetUserSkillsSnapshotBySnapshotId(snapshotId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var snapshot = await response.Content.ReadFromJsonAsync<UserSnapshotCoreDto>(_cancellationToken);

        snapshot.Should().NotBeNull();
        snapshot.Id.Should().Be(snapshotId);
        snapshot.UserSpecializations.Should().ContainSingle();
        snapshot.UserSpecializations.First().SpecializationTemplateId.Should().Be(specializationTemplateId);
    }

    [Fact]
    public async Task GetUserSnapshotByIdWhenSnapshotDoesntExistReturnsNotFound()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetUserSkillsSnapshotBySnapshotId(-1);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateUserSnapshotWhenSnapshotDateIsSetToFutureResponseWithBadRequest()
    {
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = _createUserInputFaker.Generate();

        var createdUser = await client.CreateUser(userToCreate);

        await CreateSpecializationTemplateAsync();

        var specializationTemplateId = _createdSpecializationTemplate.Id;
        var hardSkills = new HardSkillSeniorityDto[] { new() { Id = 1, SeniorityLevelId = "Junior" } };
        var userSpecializationToCreate = new CreateUserSpecializationInput(
            createdUser.Id,
            specializationTemplateId,
            "JuniorL2",
            hardSkills
        );

        await client.
            CreateUserSpecialization(userSpecializationToCreate)
            .WithResponseType<UserSpecializationCoreDto>();

        var date = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
        var createUserSnapshotInput = new CreateUserSnapshotInput(createdUser.Id, date, [
            new CreateUserSpecializationDto
            {
                SpecializationTemplateId = 1,
                HardSkills = [new HardSkillSeniorityDto { Id = 1, SeniorityLevelId = "Senior" }]
            }
        ]);

        //Act
        var response = await client.
            CreateUserSkillSnapshot(createUserSnapshotInput);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUserSnapshotWhenSnapshotDateIsLessThanExistingResponseWithConflict()
    {
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = _createUserInputFaker.Generate();

        var createdUser = await client.CreateUser(userToCreate);

        await CreateSpecializationTemplateAsync();

        var specializationTemplateId = _createdSpecializationTemplate.Id;
        var hardSkills = new HardSkillSeniorityDto[] {
            new() { Id = 1, SeniorityLevelId = "Junior" } };
        var userSpecializationToCreate = new CreateUserSpecializationInput(
            createdUser.Id,
            specializationTemplateId,
            "JuniorL2",
            hardSkills
        );

        await client.
            CreateUserSpecialization(userSpecializationToCreate)
            .WithResponseType<UserSpecializationCoreDto>();

        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        var createUserSnapshotInput = new CreateUserSnapshotInput(createdUser.Id, date,
        [
            new CreateUserSpecializationDto
            {
                SpecializationTemplateId = 1,
                SpecializationSeniorityLevelId = "Senior",
                HardSkills = [new HardSkillSeniorityDto { Id = 1, SeniorityLevelId = "Senior" }]
            }
        ]);

        //Act
        var response = await client.CreateUserSkillSnapshot(
            createUserSnapshotInput);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateUserSnapshotWhenSnapshotForThatDateAlreadyExistsResponseWithConflict()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = _createUserInputFaker.Generate();

        var createdUser = await client.CreateUser(userToCreate);

        await CreateSpecializationTemplateAsync();

        var specializationTemplateId = _createdSpecializationTemplate.Id;
        var hardSkills = new HardSkillSeniorityDto[] {
            new() { Id = 1, SeniorityLevelId = "Junior" } };
        var userSpecializationToCreate = new CreateUserSpecializationInput(
            createdUser.Id,
            specializationTemplateId,
            "JuniorL2",
            hardSkills
        );

        await client
            .CreateUserSpecialization(userSpecializationToCreate)
            .WithResponseType<UserSpecializationCoreDto>();

        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var createUserSnapshotInput = new CreateUserSnapshotInput(
            createdUser.Id, date, [
                new CreateUserSpecializationDto
                {
                    SpecializationTemplateId = 1,
                    SpecializationSeniorityLevelId = "SeniorL1",
                    HardSkills =
                    [
                        new HardSkillSeniorityDto { Id = 1, SeniorityLevelId = "Senior" }
                    ]
                }
            ]);

        //Act
        var response = await client.CreateUserSkillSnapshot(createUserSnapshotInput);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateUserSnapshotWhenRequestIsValidReturnsCreatedSnapshot()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var users = await client.GetUsersWithSearchAsHttpMessage(
            "Email eq 'snapshot.test@modsen-software.com'",
            top: 5,
            skip: 0
            ).WithResponseType<PageableListOfItems<UserOutput>>();

        var userId = users.Items.First().Id;

        var assignedSpecializations = await client.GetUserSpecializationsAsync(userId, areAssigned: true);

        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var userSpecializationDtos = assignedSpecializations!.Select(us => new CreateUserSpecializationDto
        {
            SpecializationTemplateId = us.SpecializationTemplateId,
            SpecializationSeniorityLevelId = "SeniorL2",
            HardSkills = us.HardSkills.Select(hs => new HardSkillSeniorityDto
            {
                Id = hs.Id,
                SeniorityLevelId = "Senior"
            }).ToArray()
        }).ToList();

        var createUserSnapshotInput = new CreateUserSnapshotInput(userId, date, userSpecializationDtos);

        //Act
        var response = await client
            .CreateUserSkillSnapshot(createUserSnapshotInput);

        var createdSnapshot = await response.Content.ReadFromJsonAsync<UserSnapshotCoreDto>(_cancellationToken);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        createdSnapshot.Should().NotBeNull();
        createdSnapshot.SnapshotDate.Should().Be(date);
        createdSnapshot.UserId.Should().Be(userId);
        createdSnapshot.UserSpecializations.Count.Should().Be(2);
    }
}
