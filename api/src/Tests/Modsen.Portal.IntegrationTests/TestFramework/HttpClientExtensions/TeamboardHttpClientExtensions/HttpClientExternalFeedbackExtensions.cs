using System.Net.Http.Json;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions
{
    internal static class HttpClientExternalFeedbackExtensions
    {
        private static readonly CancellationToken _cancellationToken = CancellationToken.None;

        public static Task<HttpResponseMessage> CreateExternalFeedback(this HttpClient client, CreateExternalFeedbackInput externalFeedbackToCreate)
        {
            return client.PostAsJsonAsync(
                "api/external-feedbacks", externalFeedbackToCreate, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }

        public static Task<HttpResponseMessage> UpdateExternalFeedback(this HttpClient client, UpdateExternalFeedbackInput externalFeedbackToUpdate)
        {
            return client.PutAsJsonAsync(
                "api/external-feedbacks", externalFeedbackToUpdate, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }

        public static Task<HttpResponseMessage> GetExternalFeedback(this HttpClient client, int? userId)
        {
            return client.GetAsync($"api/external-feedbacks/{userId}", _cancellationToken);
        }

        public static Task<HttpResponseMessage> GetExternalFeedback(this HttpClient client, string query)
        {
            return client.GetAsync($"api/external-feedbacks{query}", _cancellationToken);
        }
        public static Task<HttpResponseMessage> GetExternalFeedbackClients(this HttpClient client, int? userId)
        {
            return client.GetAsync($"api/external-feedbacks/clients?userId={userId}",
                _cancellationToken);
        }
        public static Task<HttpResponseMessage> GetExternalFeedbackReviewers(this HttpClient client, int? userId)
        {
            return client.GetAsync($"api/external-feedbacks/reviewers?userId={userId}",
                _cancellationToken);
        }
    }
}
