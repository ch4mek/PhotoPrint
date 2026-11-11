using System.Net.Http.Json;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions
{
    internal static class HttpClientEmploymentHistoryExtensions
    {
        private static readonly CancellationToken _cancellationToken = CancellationToken.None;

        public static Task<HttpResponseMessage> GetEmploymentHistory(this HttpClient client, int? userId)
        {
            return client.GetAsync(
                $"api/employment-history?userId={userId}",
                _cancellationToken);
        }

        public static Task<HttpResponseMessage> UpdateEmploymentHistory(this HttpClient client, UpdateEmploymentHistoryInput updateHistoryInput)
        {
            return client.PatchAsJsonAsync(
                "api/employment-history",
                updateHistoryInput,
                _cancellationToken);
        }
    }
}
