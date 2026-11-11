using System.Net.Http.Json;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions
{
    internal static class HttpClientConversationsExtensions
    {
        private static readonly CancellationToken _cancellationToken = CancellationToken.None;

        public static Task<HttpResponseMessage> CreateConversation(this HttpClient client, CreateConversationInput conversationInput)
        {
            return client.PostAsJsonAsync(
                "api/conversations", conversationInput, IntegrationTestsConstants.DefaultSerializerOptions);
        }

        public static Task<HttpResponseMessage> UpdateConversation(this HttpClient client, UpdateConversationInput conversationToUpdate)
        {
            return client.PutAsJsonAsync("api/conversations", conversationToUpdate, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }

        public static Task<HttpResponseMessage> GetConversations(this HttpClient client, string query)
        {
            return client.GetAsync($"api/conversations?{query}", _cancellationToken);
        }

    }
}
