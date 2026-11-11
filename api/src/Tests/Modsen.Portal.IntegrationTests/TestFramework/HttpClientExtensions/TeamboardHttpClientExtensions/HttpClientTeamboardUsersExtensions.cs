using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions
{
    internal static class HttpClientTeamboardUsersExtensions
    {
        private static readonly CancellationToken _cancellationToken = CancellationToken.None;

        public static Task<HttpResponseMessage> GetTeamboardUsers(this HttpClient client,
        string filter = "",
        string orderBy = "",
        int top = 25,
        int skip = 0)
        {
            var filterSection = filter.IsNullOrEmpty() ? "" : $"&filter={filter}";
            var orderbySection = orderBy.IsNullOrEmpty() ? "" : $"&orderby={orderBy}";

            return client.GetAsync($"api/teamboard-users?top={top}&skip={skip}{filterSection}{orderbySection}", _cancellationToken);
        }
        public static Task<HttpResponseMessage> GetTeamboardUsersFromQuery(this HttpClient client,
            string query)
        {
            return client.GetAsync($"api/teamboard-users{query}", _cancellationToken);
        }
        public static Task<HttpResponseMessage> GetTeamboardUsersLookup(this HttpClient client)
        {
            return client.GetAsync("api/teamboard-users/lookup", _cancellationToken);
        }

        public static Task<HttpResponseMessage> GetSaleManagersLookup(this HttpClient client,
            string query)
        {
            return client.GetAsync($"api/sale-managers/lookup{query}", _cancellationToken);
        }

        public static Task<HttpResponseMessage> GetInterviewUsersLookup(this HttpClient client,
            string query)
        {
            return client.GetAsync($"api/interview-users/lookup{query}", _cancellationToken);
        }
    }
}
