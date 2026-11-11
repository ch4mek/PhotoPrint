using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.KnowledgeBase;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.KnowledgeBase.Controllers.Contracts.Output;
namespace Modsen.Portal.IntegrationTests.KnowledgeBase.DiscountGroups;

[Collection("KnowledgeBaseCollection")]
public class DiscountGroupIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public DiscountGroupIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetDiscountGroupsResponseWithOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        // Act
        var response = await client.GetDiscountGroupsAsHttpMessage();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateDiscountGroupResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        // Act
        var response = await client.CreateDiscountGroupAsHttpMessage();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateDiscountGroupWhenUserDoesNotHasPermissionResponseWithForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var response = await client.CreateDiscountGroupAsHttpMessage();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateDiscountGroupWhenDiscountGroupWithTheSameNameAlreadyExistsResponseWithConflictStatus()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();
        var groupName = "Test group";

        var response = await client.CreateDiscountGroupAsHttpMessage(groupName);

        response.Should().NotBeNull();

        // Act
        var responseWithTheSameName = await client.CreateDiscountGroupAsHttpMessage(groupName);

        // Assert
        responseWithTheSameName.Should().NotBeNull();
        responseWithTheSameName.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetDiscountGroupResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var createdBenefitGroup = await client.CreateDiscountGroup();

        createdBenefitGroup.Should().NotBeNull();

        // Act
        var response = await client.GetDiscountGroupAsHttpMessage(createdBenefitGroup.Id);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetDiscountGroupWhenDiscountGroupDoesNotExistsResponseWithNotFoundStatus()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();
        const int nonExistingGroupId = 9999;

        // Act
        var response = await client.GetDiscountGroupAsHttpMessage(nonExistingGroupId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateDiscountGroupResponseOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        const string discountGroupName = "Test group";
        const string discountGroupUpdatedName = "Updated Name";

        var createdBenefitGroup = await client.CreateDiscountGroup(name: discountGroupName);

        createdBenefitGroup.Should().NotBeNull();

        var expectedDiscountGroupOutput = new DiscountCategoryShortOutput
        {
            Id = createdBenefitGroup.Id,
            Name = discountGroupUpdatedName,
            OrderWeight = createdBenefitGroup.OrderWeight,
        };

        // Act
        var response = await client.UpdateDiscountGroup(createdBenefitGroup.Id, discountGroupUpdatedName);

        // Assert
        response.Should().BeEquivalentTo(expectedDiscountGroupOutput);
    }

    [Fact]
    public async Task UpdateDiscountGroupWhenDiscountGroupDoesNotExistsResponseWithNotFoundStatus()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        const int nonExistingGroupId = 9999;
        const string discountGroupName = "Test group";

        // Act
        var response = await client.UpdateDiscountGroupAsHttpMessage(nonExistingGroupId, discountGroupName);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteDiscountGroupResponseOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var createdBenefitGroup = await client.CreateDiscountGroup();

        createdBenefitGroup.Should().NotBeNull();

        // Act
        var response = await client.DeleteDiscountGroupAsHttpMessage(createdBenefitGroup.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteDiscountGroupWhenDiscountGroupDoesNotExistsResponseWithNotFoundStatus()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        const int nonExistingGroupId = 9999;

        // Act
        var response = await client.DeleteDiscountGroupAsHttpMessage(nonExistingGroupId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
