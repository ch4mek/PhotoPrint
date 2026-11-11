using System.Net.Http.Json;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.Merch.Requests.Storage.Requests.RequestDto;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Merch;

internal static class HttpClientMerchStorageExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static Task<HttpResponseMessage> GetMerchStorageAsHttpMessage(this HttpClient client,
        int top,
        int skip,
        StorageQueryOptionsDto queryOptions)
    {
        return client.PostAsJsonAsync(requestUri:
            $"api/storage?top={top}&skip={skip}",
            queryOptions,
            _cancellationToken);
    }

    public static Task<PageableListOfItems<StorageItemDto>> GetMerchStorage(this HttpClient client,
        int top,
        int skip,
        StorageQueryOptionsDto queryOptions)
    {
        return client.PostAsJsonAsync<StorageQueryOptionsDto, PageableListOfItems<StorageItemDto>>(
            $"api/storage?top={top}&skip={skip}",
            queryOptions,
            _cancellationToken);
    }
}
