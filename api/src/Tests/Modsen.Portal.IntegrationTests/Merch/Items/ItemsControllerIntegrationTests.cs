using System.Net;
using System.Text.Json;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Merch;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.Merch.Requests.Items.Requests.RequestDto;
namespace Modsen.Portal.IntegrationTests.Merch.Items;

[Collection("MerchCollection")]
public class ItemsControllerIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public ItemsControllerIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetAvailableFiltersWhenUserHasViewClaimResponseWithOkResult()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        //Act
        var response = await client.GetMerchAvailableFiltersAsHttpMessage();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAvailableFiltersWhenUserHasNoViewClaimResponseWithForbiddenResult()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        //Act
        var response = await client.GetMerchAvailableFiltersAsHttpMessage();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetMerchByIdWhenUserHasNoViewClaimResponseWithForbiddenResult()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var createdItem = await client.CreateMerchItem(isClothing: true);

        await client.AuthAsEmployee();

        //Act
        var response = await client.GetMerchItemByIdAsHttpMessage(createdItem.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetMerchByIdWhenUserHasViewClaimResponseWithOkResult()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var createdItem = await client.CreateMerchItem(isClothing: true);

        //Act
        var response = await client.GetMerchItemByIdAsHttpMessage(createdItem.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMerchByIdWhenItemExistsResponseWithItem()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var createdItem = await client.CreateMerchItem(isClothing: true);

        //Act
        var item = await client.GetMerchItemById(createdItem.Id);

        // Assert
        item.Name.Should().BeEquivalentTo(item.Name);
        item.Status.Should().Be(item.Status);
        item.Description.Should().Be(item.Description);
        item.Supplier.Should().Be(item.Supplier);
        item.PhotoFileId.Should().Be(item.PhotoFileId);
        item.IsClothing.Should().Be(item.IsClothing);
    }

    [Fact]
    public async Task GetMerchByIdWhenItemNotExistsResponseWithNotFound()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        //Act
        var response = await client.GetMerchItemByIdAsHttpMessage(9999);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }

    [Fact]
    public async Task CreateMerchItemWhenRequestValidResponseWithCreatedDto()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var isClothing = false;

        var input = new CreateItemDto(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            Description: Guid.CreateVersion7().ToAlphabetic(),
            IsClothing: isClothing,
            Supplier: null,
            PhotoFileId: null,
            SizeMinAmounts: GenerateValidMinAmounts(isClothing));

        //Act
        var itemDto = await client.CreateMerchItem(input);

        // Assert
        itemDto.Name.Should().BeEquivalentTo(input.Name);
        itemDto.Description.Should().BeEquivalentTo(input.Description);
        itemDto.IsClothing.Should().Be(input.IsClothing);
        itemDto.Supplier.Should().Be(input.Supplier);
        itemDto.SizeMinAmounts.Count.Should().Be(3);

    }

    [Fact]
    public async Task CreateMerchItemWhenNotAllMinAmountsListedResponseWithNotFound()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var isClothing = false;

        var minAmounts = GenerateValidMinAmounts(isClothing);
        minAmounts.Remove(minAmounts.FirstOrDefault()!);

        var input = new CreateItemDto(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            Description: Guid.CreateVersion7().ToAlphabetic(),
            IsClothing: isClothing,
            Supplier: null,
            PhotoFileId: null,
            SizeMinAmounts: minAmounts);

        //Act
        var response = await client.CreateMerchItemAsHttpMessage(input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateMerchItemWhenItemWithSuchNameExistResponseWithBadRequest()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var name = Guid.CreateVersion7().ToAlphabetic();
        await client.CreateMerchItemAsHttpMessage(isClothing: false, name: name);

        //Act
        var response = await client.CreateMerchItemAsHttpMessage(isClothing: false, name: name);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

    }

    [Fact]
    public async Task CreateMerchItemWhenUserHasNoClaimResponseWithForbidden()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        //Act
        var response = await client.CreateMerchItemAsHttpMessage(isClothing: false);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

    }

    [Fact]
    public async Task UpdateMerchItemWhenRequestValidResponseWithUpdatedItemDto()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var isClothing = false;

        var createInput = new CreateItemDto(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            Description: Guid.CreateVersion7().ToAlphabetic(),
            IsClothing: isClothing,
            Supplier: null,
            PhotoFileId: null,
            SizeMinAmounts: GenerateValidMinAmounts(isClothing));

        var createdItem = await client.CreateMerchItem(createInput);

        var newName = Guid.CreateVersion7().ToAlphabetic();
        var newDescription = Guid.CreateVersion7().ToAlphabetic();
        var newSupplier = Guid.CreateVersion7().ToAlphabetic();

        var updateInput = new UpdateItemDto(
            Id: createdItem.Id,
            Name: newName,
            Description: newDescription,
            IsClothing: isClothing,
            Supplier: newSupplier,
            PhotoFileId: null,
            SizeMinAmounts: GenerateValidMinAmounts(isClothing));

        //Act
        var updateResponse = await client.UpdateMerchItemAsHttpMessage(updateInput);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var item = await client.GetMerchItemById(createdItem.Id);
        item.Name.Should().Be(newName);
        item.Description.Should().Be(newDescription);
        item.IsClothing.Should().Be(isClothing);
        item.Supplier.Should().Be(newSupplier);
    }

    [Fact]
    public async Task UpdateMerchItemWhenIsClothingChangedResponseWithBadRequest()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var isClothing = false;

        var createInput = new CreateItemDto(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            Description: Guid.CreateVersion7().ToAlphabetic(),
            IsClothing: isClothing,
            Supplier: null,
            PhotoFileId: null,
            SizeMinAmounts: GenerateValidMinAmounts(isClothing));

        var createdItem = await client.CreateMerchItem(createInput);

        var newName = Guid.CreateVersion7().ToAlphabetic();
        var newDescription = Guid.CreateVersion7().ToAlphabetic();
        var newSupplier = Guid.CreateVersion7().ToAlphabetic();

        var updateInput = new UpdateItemDto(
            Id: createdItem.Id,
            Name: newName,
            Description: newDescription,
            IsClothing: !isClothing,
            Supplier: newSupplier,
            PhotoFileId: null,
            SizeMinAmounts: GenerateValidMinAmounts(isClothing));

        //Act
        var updateResponse = await client.UpdateMerchItemAsHttpMessage(updateInput);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateMerchItemWhenNotAllSizesListedResponseWithNotFound()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var isClothing = false;

        var createInput = new CreateItemDto(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            Description: Guid.CreateVersion7().ToAlphabetic(),
            IsClothing: isClothing,
            Supplier: null,
            PhotoFileId: null,
            SizeMinAmounts: GenerateValidMinAmounts(isClothing));

        var createdItem = await client.CreateMerchItem(createInput);

        var newName = Guid.CreateVersion7().ToAlphabetic();
        var newDescription = Guid.CreateVersion7().ToAlphabetic();
        var newSupplier = Guid.CreateVersion7().ToAlphabetic();

        var minAmounts = GenerateValidMinAmounts(isClothing);
        minAmounts.Remove(minAmounts.FirstOrDefault()!);

        var updateInput = new UpdateItemDto(
            Id: createdItem.Id,
            Name: newName,
            Description: newDescription,
            IsClothing: isClothing,
            Supplier: newSupplier,
            PhotoFileId: null,
            SizeMinAmounts: minAmounts);

        //Act
        var updateResponse = await client.UpdateMerchItemAsHttpMessage(updateInput);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private List<SizeMinAmountDto> GenerateValidMinAmounts(bool isClothing)
    {
        return isClothing
            ?
            [
                new("XS", 1, 1),
                new("S", 1, 1),
                new("M", 1, 1),
                new("L", 1, 1),
                new("XL", 1, 1),
                new("2XL", 1, 1),
                new("3XL", 1, 1),
                new("4XL", 1, 1),

                new("XS", 2, 1),
                new("S", 2, 1),
                new("M", 2, 1),
                new("L", 2, 1),
                new("XL", 2, 1),
                new("2XL", 2, 1),
                new("3XL", 2, 1),
                new("4XL", 2, 1),

                new("XS", 3, 1),
                new("S", 3, 1),
                new("M", 3, 1),
                new("L", 3, 1),
                new("XL", 3, 1),
                new("2XL", 3, 1),
                new("3XL", 3, 1),
                new("4XL", 3, 1)
            ]
            :
            [
                new("One", 1, 1),
                new("One", 2, 1),
                new("One", 3, 1)
            ];
    }
}
