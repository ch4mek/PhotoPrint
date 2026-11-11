using System.Net;
using System.Text.Json;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Merch;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.Merch.Controllers.Contracts.Input;
using Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;
using Modsen.Portal.Merch.Requests.Storage.Requests.RequestDto;
namespace Modsen.Portal.IntegrationTests.Merch.Kits;

[Collection("MerchCollection")]
public class KitsControllerIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public KitsControllerIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task CreateKitAsyncWhenAllFieldsAreValidCreatesKit()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var firstItem = await client.CreateMerchItem(isClothing: false);
        var secondItem = await client.CreateMerchItem(isClothing: false);

        var minAmountsList = await client.GetKitsStoragesMinAmounts();

        var input = new CreateKitDto(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            PhotoFileId: "FileId",
            Description: "Description",
            Compounds: new List<AddKitCompoundDto>
            {
                new(firstItem.Id, 2),
                new(secondItem.Id, 1)
            },
            MinAmounts: minAmountsList);

        //Act
        var kitDto = await client.CreateMerchKit(input);

        // Assert
        kitDto.Id.Should().BePositive();
        kitDto.Name.Should().Be(input.Name);
        kitDto.PhotoFileId.Should().Be(input.PhotoFileId);
        kitDto.Description.Should().Be(input.Description);
        kitDto.Compounds.Count.Should().Be(input.Compounds.Count);
        kitDto.MinAmounts.Count.Should().Be(minAmountsList.Count);
    }

    [Fact]
    public async Task GetKitAsyncReturnsKit()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var firstItem = await client.CreateMerchItem(isClothing: false);
        var secondItem = await client.CreateMerchItem(isClothing: false);

        var minAmountsList = await client.GetKitsStoragesMinAmounts();

        var input = new CreateKitDto(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            PhotoFileId: "FileId",
            Description: "Description",
            Compounds: new List<AddKitCompoundDto>
            {
                new(firstItem.Id, 2),
                new(secondItem.Id, 1)
            },
            MinAmounts: minAmountsList);

        var kitId = (await client.CreateMerchKit(input)).Id;

        //Act
        var kit = await client.GetMerchKitById(kitId);

        // Assert
        kit.Name.Should().Be(input.Name);
        kit.PhotoFileId.Should().Be(input.PhotoFileId);
        kit.Description.Should().Be(input.Description);
        kit.Compounds.Count.Should().Be(input.Compounds.Count);
        kit.MinAmounts.Count.Should().Be(minAmountsList.Count);
    }

    [Fact]
    public async Task GetKitAsyncWhenKitDoesNotExistReturnsNotFound()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var kitId = 99999;

        //Act
        var httpResponse = await client.GetMerchKitByIdAsHttpMessage(kitId);

        // Assert
        httpResponse.IsSuccessStatusCode.Should().BeFalse();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateKitAsyncWhenAllFieldsAreValidUpdatesKit()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var firstItem = await client.CreateMerchItem(isClothing: false);
        var secondItem = await client.CreateMerchItem(isClothing: false);

        var minAmountsList = await client.GetKitsStoragesMinAmounts();

        var input = new CreateKitDto(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            PhotoFileId: "FileId",
            Description: "Description",
            Compounds: new List<AddKitCompoundDto>
            {
                new(firstItem.Id, 2),
                new(secondItem.Id, 1)
            },
            MinAmounts: minAmountsList);

        var kitId = (await client.CreateMerchKit(input)).Id;

        var updateInput = new UpdateKitInput(
            Name: "New name",
            PhotoFileId: "ffffffffff",
            Description: "new description",
            Compounds: new List<AddKitCompoundDto>
            {
                new(firstItem.Id, 5),
                new(secondItem.Id, 2)
            },
            MinAmounts: minAmountsList);

        //Act
        var updateResponse = await client.UpdateMerchKitAsHttpMessage(kitId, updateInput);

        // Assert
        updateResponse.IsSuccessStatusCode.Should().BeTrue();

        var kit = await client.GetMerchKitById(kitId);

        kit.Name.Should().Be(updateInput.Name);
        kit.Description.Should().Be(updateInput.Description);
        kit.PhotoFileId.Should().Be(updateInput.PhotoFileId);

        kit.Compounds.First().AmountInKit.Should().Be(5);
        kit.Compounds.Skip(1).First().AmountInKit.Should().Be(2);
    }

    [Fact]
    public async Task GetPageableMerchKitsReturnsKits()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var firstItem = await client.CreateMerchItem(isClothing: false);
        var secondItem = await client.CreateMerchItem(isClothing: false);

        var minAmountsList = await client.GetKitsStoragesMinAmounts();

        var input = new CreateKitDto(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            PhotoFileId: Guid.CreateVersion7().ToAlphabetic(),
            Description: Guid.CreateVersion7().ToAlphabetic(),
            Compounds: new List<AddKitCompoundDto>
            {
                new(firstItem.Id, 2),
                new(secondItem.Id, 1)
            },
            MinAmounts: minAmountsList);
        var createdKitId = (await client.CreateMerchKit(input)).Id;

        //Act
        var kits = await client.GetPageableMerchKits(
            queryOptions: new KitsQueryOptionsDto
            {
                Filters =
                [
                    new QueryFilterDto {FilterColumn = "id", FilterValue = $"{createdKitId}"}
                ]
            },
            top: 100,
            skip: 0);

        // Assert
        kits.Items.Count.Should().Be(1);
        kits.Items.Select(kit => kit.Id).ToList().Should().Contain(createdKitId);
    }
}
