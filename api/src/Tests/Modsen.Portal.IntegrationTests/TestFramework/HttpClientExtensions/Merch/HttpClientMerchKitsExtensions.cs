using System.Net.Http.Json;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.Merch.Controllers.Contracts.Input;
using Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Merch;

internal static class HttpClientMerchKitsExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static Task<PageableListOfItems<KitStorageDto>> GetPageableMerchKits(this HttpClient client,
        KitsQueryOptionsDto queryOptions,
        int top = 10,
        int skip = 0)
    {
        return client.PostAsJsonAsync<KitsQueryOptionsDto, PageableListOfItems<KitStorageDto>>(
            $"api/kits/search?top={top}&skip={skip}",
            queryOptions,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetPageableMerchKitsAsHttpMessage(this HttpClient client,
        KitsQueryOptionsDto queryOptions,
        int top = 10,
        int skip = 0)
    {
        return client.PostAsJsonAsync($"api/kits/search?top={top}&skip={skip}",  queryOptions,
            _cancellationToken);
    }

    public static Task<KitDto> GetMerchKitById(this HttpClient client, int id)
    {
        return client.GetFromJsonAsync<KitDto>(
            $"api/kits/{id}",
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetMerchKitByIdAsHttpMessage(this HttpClient client, int id)
    {
        return client.GetAsync($"api/kits/{id}", _cancellationToken);
    }

    public static Task<KitDto> UpdateMerchKit(this HttpClient client, int id, UpdateKitInput input)
    {
        return client.PutAsJsonAsync<UpdateKitInput, KitDto>($"api/kits/{id}", input, _cancellationToken);
    }

    public static Task<HttpResponseMessage> UpdateMerchKitAsHttpMessage(this HttpClient client, int id, UpdateKitInput input)
    {
        return client.PutAsJsonAsync($"api/kits/{id}", input, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateMerchKitAsHttpMessage(this HttpClient client,
        CreateKitDto input)
    {
        return client.PostAsJsonAsync("api/kits", input, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<KitDto> CreateMerchKit(this HttpClient client, CreateKitDto input)
    {
        return client.PostAsJsonAsync<CreateKitDto, KitDto>("api/kits", input, _cancellationToken);
    }

    public static Task<KitDto> CreateMerchKit(this HttpClient client, List<AddKitCompoundDto> compounds,
        List<KitMinAmountDto> minAmounts,
        string? name = null,
        string? description = null,
        string? photoFileId = null)
    {
        var input = new CreateKitDto(
            Name: name ?? Guid.CreateVersion7().ToAlphabetic(),
            Description: description ?? Guid.CreateVersion7().ToAlphabetic(),
            PhotoFileId: photoFileId ?? Guid.CreateVersion7().ToAlphabetic(),
            Compounds: compounds,
            MinAmounts: minAmounts);

        return client.PostAsJsonAsync<CreateKitDto, KitDto>("api/kits", input, _cancellationToken);
    }

    public static async Task<List<KitMinAmountDto>> GetKitsStoragesMinAmounts(this HttpClient client)
    {
        var config = await client.GetConfiguration();
        var storages = config?.ConfigurableValues.MerchStorages ?? [];
        var minAmountsList = storages.Select(merchStorage => new KitMinAmountDto(merchStorage.Id, 1)).ToList();

        return minAmountsList;
    }
}
