using System.Net.Http.Json;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions
{
    internal static class HttpClientContractsExtensions
    {
        private static readonly CancellationToken _cancellationToken = CancellationToken.None;

        public static Task<HttpResponseMessage> CreateContracts(this HttpClient client,
        CreateUserContractInput input)
        {
            return client.PostAsJsonAsync(
                "api/contracts", input, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }

        public static Task<HttpResponseMessage> GetActiveExpiringContracts(this HttpClient client)
        {
            return client.GetAsync(
                requestUri: "api/contracts/active-expiring", cancellationToken: _cancellationToken);
        }

        public static Task<HttpResponseMessage> UpdateContracts(this HttpClient client,
            UpdateUserContractInput input)
        {
            return client.PutAsJsonAsync(
                "api/contracts", input, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }

        public static Task<HttpResponseMessage> GetContracts(this HttpClient client,
            string query)
        {
            return client.GetAsync(
                $"api/contracts?{query}", _cancellationToken);
        }

        public static Task<HttpResponseMessage> GetContractById(this HttpClient client,
            int id)
        {
            return client.GetAsync(
                $"api/contracts/{id}", _cancellationToken);
        }
    }
}
