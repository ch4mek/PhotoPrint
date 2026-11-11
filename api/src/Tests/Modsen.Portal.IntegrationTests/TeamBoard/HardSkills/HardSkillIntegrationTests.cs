using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.CoreDto;
namespace Modsen.Portal.IntegrationTests.TeamBoard.HardSkills;

[Collection("GeneralCollection")]
public class HardSkillIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public HardSkillIntegrationTests(IntegrationTestFactory factory)
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
    [InlineData("?top=50&skip=50&filter=Id eq ll")]
    public async Task GetHardSkillsWhenRequestIsInvalidResponseWithBadRequestResult(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetHardSkills(query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("?top=10&skip=0")]
    [InlineData("?top=10&skip=50")]
    [InlineData("?top=50&skip=50&filter=contains(Name,'Skill')")]
    [InlineData("?top=50&skip=50&orderby=Id")]
    [InlineData("?top=50&skip=50&orderby=Name asc")]
    [InlineData("?top=50&skip=50&orderby=Name desc")]
    public async Task GetHardSkillsWhenRequestIsValidResponseWithOk(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetHardSkills(query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetHardSkillsWhenSpecializationQueryParamSpecifiedResponseExpectedOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var firstHardSkillInput = new CreateHardSkillInput(Guid.CreateVersion7().ToAlphabetic(), "CID");

        var firstHardSkill = await client.CreateHardSkills(
            firstHardSkillInput)
            .WithResponseType<HardSkillOutput>();

        var secondHardSkillInput = new CreateHardSkillInput(Guid.CreateVersion7().ToAlphabetic(), "CID");

        await client.CreateHardSkills(
                secondHardSkillInput)
            .WithResponseType<HardSkillOutput>();

        var createSpecializationInput = new CreateSpecializationTemplateInput(Guid.CreateVersion7().ToAlphabetic(),
            DepartmentIds: new List<string>(), HardSkillids: new List<int>());

        var createdSpecialization = await client.CreateSpecializationTemaplate(
            createSpecializationInput)
            .WithResponseType<SpecializationTemplateOutput>();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TeamBoardDbContext>();

        var specializationTemplateHardSkill = new SpecializationTemplateHardSkill
        {
            HardSkillId = firstHardSkill.Id,
            SpecializationTemplateId = createdSpecialization.Id,
            CreatedById = _factory.SystemUserId!.Value,
            CreatedAtUtc = DateTime.UtcNow.Date
        };

        await dbContext.SpecializationTemplateHardSkills.AddAsync(specializationTemplateHardSkill, _cancellationToken);

        await dbContext.SaveChangesAsync(_cancellationToken);

        var query = $"?specializationTemplateId={createdSpecialization.Id}&top=100&skip=0";

        var expectedHardSkillOutput = new OdpHardSkill
        {
            Id = firstHardSkill.Id,
            Name = firstHardSkill.Name,
            HardSkillGroupId = "CID"
        };

        // Act
        var response = await client.GetHardSkills(query)
            .WithResponseType<PageableListOfItems<OdpHardSkill>>();

        // Assert
        response.Items.Should().ContainSingle();
        response.Items.First().Should().BeEquivalentTo(expectedHardSkillOutput);
    }

    [Fact]
    public async Task GetHardSkillsWhenDepartmentQueryParamSpecifiedResponseExpectedOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var firstHardSkillInput = new CreateHardSkillInput(Guid.CreateVersion7().ToAlphabetic(), "CID");

        var firstHardSkill = await client.CreateHardSkills(
            firstHardSkillInput)
            .WithResponseType<HardSkillOutput>();

        var secondHardSkillInput = new CreateHardSkillInput(Guid.CreateVersion7().ToAlphabetic(), "CID");

        await client.CreateHardSkills(
                secondHardSkillInput)
            .WithResponseType<HardSkillOutput>();

        var createSpecializationInput = new CreateSpecializationTemplateInput(Guid.CreateVersion7().ToAlphabetic(),
            DepartmentIds: new List<string>(), HardSkillids: new List<int>());

        var createdSpecialization = await client.CreateSpecializationTemaplate(createSpecializationInput)
            .WithResponseType<SpecializationTemplateOutput>();

        var department = "DevOps";

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TeamBoardDbContext>();

        var specializationTemplateDepartment = new SpecializationTemplateDepartment
        {
            DepartmentId = department,
            SpecializationTemplateId = createdSpecialization.Id,
            CreatedById = _factory.SystemUserId!.Value,
            CreatedAtUtc = DateTime.UtcNow.Date
        };

        var specializationTemplateHardSkill = new SpecializationTemplateHardSkill
        {
            HardSkillId = firstHardSkill.Id,
            SpecializationTemplateId = createdSpecialization.Id,
            CreatedById = _factory.SystemUserId!.Value,
            CreatedAtUtc = DateTime.UtcNow.Date
        };

        await dbContext.SpecializationTemplateHardSkills.AddAsync(specializationTemplateHardSkill, _cancellationToken);

        await dbContext.SpecializationTemplateDepartments.AddAsync(specializationTemplateDepartment, _cancellationToken);

        await dbContext.SaveChangesAsync(_cancellationToken);

        var query = $"?departmentId={department}&top=100&skip=0";

        var expectedHardSkillOutput = new OdpHardSkill
        {
            Id = firstHardSkill.Id,
            Name = firstHardSkill.Name,
            HardSkillGroupId = "CID"
        };

        // Act
        var response = await client.GetHardSkills(query)
            .WithResponseType<PageableListOfItems<OdpHardSkill>>();

        // Assert
        response.Items.Should().ContainSingle();
        response.Items.First().Should().BeEquivalentTo(expectedHardSkillOutput);
    }

    [Fact]
    public async Task DeleteHardSkillWhenHardSkillExistsReturnOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();

        await client.AuthAsCeo();

        var createHardSkillInput = new CreateHardSkillInput("NewHardSkill", "CID");

        var createdHardSkill = await client.CreateHardSkills(
            createHardSkillInput).WithResponseType<HardSkillOutput>();

        //Act
        var response = await client.DeleteHardSkills(createdHardSkill.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var deletedHardSkill = await response.Content.ReadFromJsonAsync<HardSkillOutput>(_cancellationToken);

        deletedHardSkill.Should().BeEquivalentTo(createdHardSkill);
    }

    [Fact]
    public async Task DeleteHardSkillWhenItsLastHardSkillInUserSpecializationReturnConflict()
    {
        // Arrange
        var client = _factory.CreateClient();

        await client.AuthAsCeo();

        var createHardSkillInput = new CreateHardSkillInput("TestCannotBeDeleted", "CID");

        var createdHardSkill = await client.CreateHardSkills(
            createHardSkillInput)
            .WithResponseType<HardSkillOutput>();

        var departmentIds = new List<string> { };
        var hardSkillIds = new List<int> { createdHardSkill.Id };
        var hardSkillsSeniroties = new HardSkillSeniorityDto[]
        {
            new()
            {
                Id = createdHardSkill.Id,
                SeniorityLevelId = "Junior"
            }
        };

        var specializationToCreate = new CreateSpecializationTemplateInput(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            departmentIds,
            hardSkillIds
        );

        var createdSpecializationTemplate = await client.CreateSpecializationTemaplate(
            specializationToCreate)
            .WithResponseType<SpecializationTemplateOutput>();

        var primarySpecializationToCreate = new CreateUserSpecializationInput(
            _factory.EmployeeUserId.Value,
            createdSpecializationTemplate.Id,
            "JuniorL2",
            hardSkillsSeniroties
        );

        await client.CreateUserSpecialization(
                primarySpecializationToCreate)
            .WithResponseType<UserSpecializationCoreDto>();

        //Act
        var response = await client.DeleteHardSkills(createdHardSkill.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetHardSkillByIdWhenHardSkillIsAlreadyDeletedReturnNotFoundResult()
    {
        // Arrange
        var client = _factory.CreateClient();

        await client.AuthAsCeo();

        var createHardSkillInput = new CreateHardSkillInput("NewHardSkill2", "CID");

        var createdHardSkill = await client.CreateHardSkills(
            createHardSkillInput).WithResponseType<HardSkillOutput>();

        await client.DeleteHardSkills(createdHardSkill.Id);

        //Act
        var response = await client.GetHardSkills(createdHardSkill.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateHardSkillWhenRequestIsValidReturnOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();

        await client.AuthAsCeo();

        var createHardSkillInput = new CreateHardSkillInput(Guid.CreateVersion7().ToAlphabetic(), "CID");

        var createdHardSkill = await client.CreateHardSkills(
            createHardSkillInput)
            .WithResponseType<HardSkillOutput>();

        var updatedName = Guid.CreateVersion7().ToAlphabetic();

        var updateHardSkillInput = new UpdateHardSkillInput(createdHardSkill.Id, updatedName, "CID");

        var expectedHardSkillOutput = new HardSkillOutput(createdHardSkill.Id, updatedName, "CID");

        //Act
        var response = await client.UpdateHardSkills(
            updateHardSkillInput)
            .WithResponseType<HardSkillOutput>();

        // Assert
        response.Should().BeEquivalentTo(expectedHardSkillOutput);
    }

    [Fact]
    public async Task UpdateHardSkillWhenNothingChangedReturnOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();

        await client.AuthAsCeo();

        var createHardSkillInput = new CreateHardSkillInput(Guid.CreateVersion7().ToAlphabetic(), "CID");

        var createdHardSkill = await client.CreateHardSkills(
            createHardSkillInput)
            .WithResponseType<HardSkillOutput>();

        var updatedName = $"   {createHardSkillInput.Name}   ";

        var updateHardSkillInput = new UpdateHardSkillInput(createdHardSkill.Id, updatedName, "CID");

        var expectedHardSkillOutput = new HardSkillOutput(createdHardSkill.Id, createHardSkillInput.Name, "CID");

        //Act
        var response = await client.UpdateHardSkills(
            updateHardSkillInput)
            .WithResponseType<HardSkillOutput>();

        // Assert
        response.Should().BeEquivalentTo(expectedHardSkillOutput);
    }

    [Fact]
    public async Task GetHardSkillsLookupWhenRequestIsValidResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetHardSkillsLookup()
            .WithResponseType<List<HardSkillLookupOutput>>();

        // Assert
        response.Count.Should().BeGreaterThan(0);
        response[0].Should().NotBeNull();
        response[0].Name.Should().NotBeEmpty();
    }
}
