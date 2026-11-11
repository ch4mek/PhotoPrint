using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.KnowledgeBase;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.KnowledgeBase.Controllers.Contracts.Output;
namespace Modsen.Portal.IntegrationTests.KnowledgeBase.Discounts;

[Collection("KnowledgeBaseCollection")]
public class DiscountsIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public DiscountsIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task SearchDiscountsAsyncReturnsOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var discountGroup = await client.CreateDiscountGroup();

        await client.CreateDiscountAsHttpMessage(
            discountCategoryId: discountGroup.Id,
            discountCityIds: [1]);

        // Act
        var discountsResponse = await client.GetDiscountsAsHttpMessage();

        // Assert
        discountsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetDiscountsByGroupAsyncReturnsOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var discountGroup = await client.CreateDiscountGroup();
        var discountGroupId = discountGroup.Id;

        await client.CreateDiscountAsHttpMessage(
            discountCategoryId: discountGroupId,
            discountCityIds: [1]);

        await client.CreateDiscountAsHttpMessage(
            discountCategoryId: discountGroupId,
            discountCityIds: [1]);

        // Act
        var discountsResponse = await client.GetDiscountsByGroup(discountGroupId);

        // Assert
        discountsResponse.Count.Should().Be(2);
    }

    [Fact]
    public async Task CreateDiscountReturnsOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var discountGroup = await client.CreateDiscountGroup();
        var discountGroupId = discountGroup.Id;

        // Act
        var discountResponse = await client.CreateDiscountAsHttpMessage(
            discountCategoryId: discountGroupId,
            discountCityIds: [1],
            discountSocialMediaUrls: [Guid.CreateVersion7().ToAlphabetic()]);

        // Assert
        discountResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateDiscountReturnsOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var discountGroup = await client.CreateDiscountGroup();
        var discountGroupId = discountGroup.Id;

        var createdDiscount = await client.CreateDiscount(
            discountCategoryId: discountGroupId,
            discountCityIds: [1],
            discountSocialMediaUrls: [Guid.CreateVersion7().ToAlphabetic()]);

        const string newName = "New";
        const string newText = "New text";
        const string newImageUrl = "path.file";
        const string newSocialMediaUrl = "www.instagram.com";
        const bool newIsActive = false;
        const int newPercent = 15;

        // Act
        var response = await client.UpdateDiscountAsHttpMessage(
            id: createdDiscount.Id,
            discountCategoryId: discountGroupId,
            text: newText,
            discountPercent: newPercent,
            imageUrl: newImageUrl,
            name: newName,
            isActive: newIsActive,
            discountCityIds: [1],
            discountSocialMediaUrls: [newSocialMediaUrl]);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await response.Content.ReadFromJsonAsync<DiscountOutput>();

        updated.DiscountCategoryName.Should().Be(discountGroup.Name);
        updated.Name.Should().Be(newName);
        updated.Text.Should().Be(newText);
        updated.ImageUrl.Should().Be(newImageUrl);
        updated.DiscountPercent.Should().Be(newPercent);
        updated.IsActive.Should().Be(newIsActive);
        updated.DiscountTags.Select(x => x.TagId).Should().BeEquivalentTo(Array.Empty<int>());
        updated.DiscountCities.Select(x => x.CityId).Should().BeEquivalentTo([1]);
    }

    [Fact]
    public async Task UpdateDiscountGroupResponseOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var discountGroup = await client.CreateDiscountGroup();
        var discountGroupId = discountGroup.Id;

        var createdDiscount = await client.CreateDiscount(
            discountCategoryId: discountGroupId,
            discountCityIds: [1],
            discountSocialMediaUrls: [Guid.CreateVersion7().ToAlphabetic()]);

        createdDiscount.Should().NotBeNull();

        const string newName = "OZ discount";
        const string newSocialMediaUrl = "www.instagram.com";

        // Act
        var updatedDiscountResponse = await client.UpdateDiscount(
            id: createdDiscount.Id,
            discountCategoryId: discountGroupId,
            name: newName,
            discountCityIds: [1],
            discountSocialMediaUrls: [newSocialMediaUrl]);

        // Assert
        updatedDiscountResponse.Name.Should().Be(newName);
    }

    [Fact]
    public async Task DeleteDiscountGroupResponseOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var discountGroup = await client.CreateDiscountGroup();
        var discountGroupId = discountGroup.Id;

        var createdDiscount = await client.CreateDiscount(
            discountCategoryId: discountGroupId,
            discountCityIds: [1],
            discountSocialMediaUrls: [Guid.CreateVersion7().ToAlphabetic()]);

        // Act
        var deleteDiscountResponse = await client.DeleteDiscountAsHttpMessage(createdDiscount.Id);

        // Assert
        deleteDiscountResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
