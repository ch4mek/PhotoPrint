using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
namespace Modsen.Portal.IntegrationTests.TeamBoard.SoftSkills;

[Collection("GeneralCollection")]
public class SoftSkillIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public SoftSkillIntegrationTests(IntegrationTestFactory factory)
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
    public async Task GetSoftSkillsWhenRequestIsInvalidResponseWithBadRequestResult(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetSoftSkills(query);

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
    public async Task GetSoftSkillsWhenRequestIsValidResponseWithOk(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetSoftSkills(query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateSoftSkillWhenSoftSkillWithTheSameNameAlreadyExistsReturnConflictStatus()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var softSkillToCreate = new CreateSoftSkillInput(Name: "Initiative");

        var softSkillToCreate2 = new CreateSoftSkillInput(Name: "   Initiative  ");

        await client.CreateSoftSkills(softSkillToCreate);

        // Act
        var response = await client.CreateSoftSkills(softSkillToCreate2);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetSoftSkillsWhenSoftSkillDoesNotExistShouldReturnNotFoundResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var result = await client.GetSoftSkillsById(9999999);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateSoftSkillWhenRequestIsInvalidReturnBadRequestStatus()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var softSkillToCreate = new CreateSoftSkillInput(Name: "  ---  ");

        // Act
        var response = await client.CreateSoftSkills(softSkillToCreate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task GetSoftSkillsWhenRequestContainsValidFilterResponseWithSoftSkills()
    {
        // Arrange
        var client = _factory.CreateClient();

        await client.AuthAsCeo();

        await client.CreateSoftSkills(
            new CreateSoftSkillInput(
                Name: "Skill1"
            ));

        await client.CreateSoftSkills(
            new CreateSoftSkillInput(
                Name: "Skill2"
            ));

        await client.CreateSoftSkills(
            new CreateSoftSkillInput(
                Name: "Test"
            ));

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.GetSoftSkills("?top=20&skip=0&filter=contains(Name,'Skill')");
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PageableListOfItems<OdpSoftSkill>>(_cancellationToken);
        body.Should().NotBeNull();
        body.Total.Should().BeGreaterThanOrEqualTo(2);
        body.Items.All(s => s.Name.Contains("Skill")).Should().BeTrue();
    }

    [Fact]
    public async Task GetSoftSkillsWhenRequestContainsValidFilterAuthAsGroupManagerResponseWithSoftSkills()
    {
        // Arrange
        var client = _factory.CreateClient();

        await client.AuthAsCeo();

        await client.CreateSoftSkills(
            new CreateSoftSkillInput(
                Name: "SkillToTestGM1"
            ));

        await client.CreateSoftSkills(
            new CreateSoftSkillInput(
                Name: "SkillToTestGM2"
            ));

        await client.CreateSoftSkills(
            new CreateSoftSkillInput(
                Name: "SkillToTestGM3"
            ));

        await client.AuthAsGroupManager();

        // Act
        var response = await client.GetSoftSkills("?top=20&skip=0&filter=contains(Name,'TestGM')");
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PageableListOfItems<OdpSoftSkill>>(_cancellationToken);
        body.Should().NotBeNull();
        body.Total.Should().BeGreaterThanOrEqualTo(2);
        body.Items.All(s => s.Name.Contains("TestGM")).Should().BeTrue();
    }

    [Fact]
    public async Task GetSoftSkillsByPostWhenRequestContainsValidFilterResponseWithSoftSkills()
    {
        // Arrange
        var client = _factory.CreateClient();

        await client.AuthAsCeo();

        await client.CreateSoftSkills(
            new CreateSoftSkillInput(
                Name: "PostSkill1"
            ));

        await client.CreateSoftSkills(
            new CreateSoftSkillInput(
                Name: "PostSkill2"
            ));

        await client.CreateSoftSkills(
            new CreateSoftSkillInput(
                Name: "GetTest"
            ));

        await client.AuthAsHeadOfDepartment();


        using var content = new StringContent("$filter=contains(Name,'Post')&$orderby Id desc", Encoding.UTF8, "text/plain");
        // Act
        var response = await client.GetSoftSkills("/$query?$top=10&$skip=0", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PageableListOfItems<OdpSoftSkill>>(_cancellationToken);
        body.Should().NotBeNull();
        body.Total.Should().BeGreaterThanOrEqualTo(2);
        body.Items.All(s => s.Name.Contains("Post")).Should().BeTrue();
    }

    [Fact]
    public async Task GetSoftSkillsByPostWhenRequestContainsValidFilterAuthAsGroupManagerResponseWithSoftSkills()
    {
        // Arrange
        var client = _factory.CreateClient();

        await client.AuthAsCeo();

        await client.CreateSoftSkills(
            new CreateSoftSkillInput(
                Name: "SKGMTES1234dfgdfT11"
            ));

        await client.CreateSoftSkills(
            new CreateSoftSkillInput(
                Name: "SKGMTES1234sdfT12"
            ));

        await client.CreateSoftSkills(
            new CreateSoftSkillInput(
                Name: "SomeSkillfdsadfT1-1"
            ));

        await client.AuthAsGroupManager();

        // Act

        using var content = new StringContent("$filter=contains(Name,'SKGM')&$orderby Id desc", Encoding.UTF8, "text/plain");
        var response = await client.GetSoftSkills("/$query?$top=10&$skip=0", content);
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PageableListOfItems<OdpSoftSkill>>(_cancellationToken);
        body.Should().NotBeNull();
        body.Total.Should().BeGreaterThanOrEqualTo(2);
        body.Items.All(s => s.Name.Contains("SKGM")).Should().BeTrue();
    }

    [Fact]
    public async Task DeleteSoftSkillWhenSoftSkillExistResponseWithSoftSkill()
    {
        // Arrange
        var client = _factory.CreateClient();

        await client.AuthAsCeo();

        var createdSoftSkill = await client.CreateSoftSkills(
            new CreateSoftSkillInput(
                Name: "SuperSkill"
            )).WithResponseType<SoftSkillOutput>();

        var softSkillToDeleteId = createdSoftSkill.Id;

        // Act
        var response = await client.DeleteSoftSkills(softSkillToDeleteId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<SoftSkillOutput>(_cancellationToken);
        body.Id.Should().Be(softSkillToDeleteId);
        body.Name.Should().Be(createdSoftSkill.Name);
    }

    [Fact]
    public async Task UpdateSoftSkillWhenNothingChangedReturnOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();

        await client.AuthAsCeo();

        var createSoftSkillInput = new CreateSoftSkillInput(Guid.CreateVersion7().ToAlphabetic());

        var createdSoftSkill = await client.CreateSoftSkills(
            createSoftSkillInput)
            .WithResponseType<SoftSkillOutput>();

        var updatedName = $"   {createSoftSkillInput.Name}   ";

        var updateSoftSkillInput = new UpdateSoftSkillInput(createdSoftSkill.Id, updatedName);

        var expectedSoftSkillOutput = new SoftSkillOutput(createdSoftSkill.Id, createSoftSkillInput.Name);

        //Act
        var response = await client.UpdateSoftSkills(updateSoftSkillInput)
            .WithResponseType<SoftSkillOutput>();

        // Assert
        response.Should().BeEquivalentTo(expectedSoftSkillOutput);
    }
}
