using System.Net.Http.Json;
using Modsen.Portal.Merch.Controllers.Contracts.Input;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Merch
{
    internal static class HttpClientTransactionsExtensions
    {
        internal static readonly CancellationToken _cancellationToken = CancellationToken.None;

        public static Task<HttpResponseMessage> CreateItemTransaction(this HttpClient client, AddItemTransactionInput input)
        {
            return client.PostAsJsonAsync(requestUri:
                "api/itemtransactions", input,
                IntegrationTestsConstants.DefaultSerializerOptions,
                _cancellationToken);
        }

        public static Task<HttpResponseMessage> CreateKitTransaction(this HttpClient client,
            AddKitTransactionInput input)
        {
            return client.PostAsJsonAsync(requestUri:
                "api/itemtransactions/kit", input,
                IntegrationTestsConstants.DefaultSerializerOptions,
                _cancellationToken);
        }

        public static Task<HttpResponseMessage> GetTransactions(this HttpClient client,
            int top = 25,
            int skip = 0,
            string filter = "",
            TransactionFilters Idfilters = null)
        {
            var query = $"api/itemtransactions/search?Top={top}&Skip={skip}";
            query +=  !string.IsNullOrEmpty(filter) ? $"&filter{filter}" : string.Empty;

            return client.PostAsJsonAsync(requestUri:
                query, Idfilters,
                _cancellationToken);
        }
    }
}
