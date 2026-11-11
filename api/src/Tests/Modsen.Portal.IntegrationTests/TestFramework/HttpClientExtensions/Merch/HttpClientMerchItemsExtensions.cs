using System.Net.Http.Json;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.Merch.Requests.Items.Requests.RequestDto;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Merch;

internal static class HttpClientMerchItemsExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static Task<HttpResponseMessage> GetMerchAvailableFiltersAsHttpMessage(this HttpClient client)
    {
        return client.GetAsync("api/items/filters", _cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateMerchItemAsHttpMessage(this HttpClient client,
        CreateItemDto input)
    {
        return client.PostAsJsonAsync("api/items", input, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<ItemDto> CreateMerchItem(this HttpClient client, CreateItemDto input)
    {
        return client.PostAsJsonAsync<CreateItemDto, ItemDto>("api/items", input, _cancellationToken);
    }

    public static async Task<ItemDto> CreateMerchItem(this HttpClient client,
        bool isClothing,
        string? name = null,
        string? description = null,
        string? supplier = null,
        List<SizeMinAmountDto>? minAmounts = null)
    {
        var input = new CreateItemDto(
            Name: name ?? Guid.CreateVersion7().ToAlphabetic(),
            Description : description,
            IsClothing : isClothing,
            Supplier : supplier,
            PhotoFileId : null,
            SizeMinAmounts : minAmounts ?? (isClothing ?
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
                ])
        );

        return await client.PostAsJsonAsync<CreateItemDto, ItemDto>("api/items", input, _cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateMerchItemAsHttpMessage(this HttpClient client,
        bool isClothing,
        string? name = null,
        string? description = null,
        string? supplier = null,
        List<SizeMinAmountDto>? minAmounts = null)
    {
        var input = new CreateItemDto(
            Name: name ?? Guid.CreateVersion7().ToAlphabetic(),
            Description : description,
            IsClothing : isClothing,
            Supplier : supplier,
            PhotoFileId : null,
            SizeMinAmounts : minAmounts ?? (isClothing ?
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
                ])
        );

        return client.PostAsJsonAsync("api/items", input, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<ItemDto> UpdateMerchItem(this HttpClient client, UpdateItemDto input)
    {
        return client.PutAsJsonAsync<UpdateItemDto, ItemDto>("api/items", input, _cancellationToken);
    }

    public static Task<HttpResponseMessage> UpdateMerchItemAsHttpMessage(this HttpClient client, UpdateItemDto input)
    {
        return client.PutAsJsonAsync("api/items", input, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<ItemDto> GetMerchItemById(this HttpClient client, int id)
    {
        return client.GetFromJsonAsync<ItemDto>(
            $"api/items/{id}",
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetMerchItemByIdAsHttpMessage(this HttpClient client, int id)
    {
        return client.GetAsync($"api/items/{id}", _cancellationToken);
    }

    public static Task<List<ItemLookupDto>> GetMerchItemsLookupById(this HttpClient client)
    {
        return client.GetFromJsonAsync<List<ItemLookupDto>>(
            "api/items/lookup",
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetMerchItemsLookupAsHttpMessage(this HttpClient client)
    {
        return client.GetAsync("api/items/lookup", _cancellationToken);
    }
}
