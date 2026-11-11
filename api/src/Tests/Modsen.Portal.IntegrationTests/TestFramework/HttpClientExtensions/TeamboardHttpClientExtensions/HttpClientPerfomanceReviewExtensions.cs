using System.Net.Http.Json;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Requests.RequestDto;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions
{
    internal static class HttpClientPerfomanceReviewExtensions
    {
        private static readonly CancellationToken _cancellationToken = CancellationToken.None;

        public static Task<HttpResponseMessage> CreatePerfomanceReview(this HttpClient client, CreatePerformanceReviewDto createPerformanceReviewInput)
        {
            return client.PostAsJsonAsync(
                "api/performance-review", createPerformanceReviewInput, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }
        public static Task<HttpResponseMessage> GetPerfomanceReview(this HttpClient client, string query)
        {
            return client.GetAsync(
                $"api/performance-review{query}",
                _cancellationToken);
        }

        public static Task<HttpResponseMessage> GetPerformanceReviewByUserId(this HttpClient client, int userId, int overdueDaysCount, string query)
        {
            return client.GetAsync(
                $"api/performance-review/user/?overdueDaysCount={overdueDaysCount}&$filter=((userId eq {userId}))&{query}",
                _cancellationToken);
        }

        public static Task<HttpResponseMessage> GetPerformanceReviewLookupAsHttpResponseMessageAsync(
            this HttpClient client, int overdueDays = 7, string? departmentId = null)
        {
            var queryParams = departmentId == null
                ? $"overdueDays={overdueDays}"
                : $"overdueDays={overdueDays}&departmentId={departmentId}";

            return client.GetAsync($"api/performance-review/lookup?{queryParams}", _cancellationToken);
        }

        public static Task<PerformanceReviewLookupDto[]> GetPerformanceReviewLookupAsync(this HttpClient client,
            int overdueDays = 7, string? departmentId = null)
        {
            var queryParams = departmentId == null
                ? $"overdueDays={overdueDays}"
                : $"overdueDays={overdueDays}&departmentId={departmentId}";

            return client.GetFromJsonAsync<PerformanceReviewLookupDto[]>(
                $"api/performance-review/lookup?{queryParams}", _cancellationToken)!;
        }
    }
}
