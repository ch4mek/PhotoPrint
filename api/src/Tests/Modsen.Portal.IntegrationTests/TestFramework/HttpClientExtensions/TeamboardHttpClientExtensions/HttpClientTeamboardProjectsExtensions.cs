using System.Net.Http.Json;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions
{
    internal static class HttpClientTeamboardProjectsExtensions
    {
        private static readonly CancellationToken _cancellationToken = CancellationToken.None;

        public static Task<HttpResponseMessage> GetUserProjects(this HttpClient client,
        int userId)
        {
            return client.GetAsync($"api/user-projects/lookup?userId={userId}", _cancellationToken);
        }

        public static Task<HttpResponseMessage> GetUserProjectsByAssignementId(this HttpClient client,
            int assignementId)
        {
            return client.GetAsync($"api/user-projects/{assignementId}", _cancellationToken);
        }

        public static Task<HttpResponseMessage> GetUserProjects(this HttpClient client,
            string query)
        {
            return client.GetAsync($"api/user-projects?{query}", _cancellationToken);
        }
        public static Task<HttpResponseMessage> CreateProjectAsHttpResponse(this HttpClient client,
            CreateProjectInput input)
        {
            return client.PostAsJsonAsync("api/projects", input, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }

        public static Task<HttpResponseMessage> CreateProjectAssignment(this HttpClient client,
            int userId, int projectId, CreateUserProjectAssignmentInput input)
        {
            return client.PostAsJsonAsync(
                $"api/users/{userId}/projects/{projectId}",
                input,
                _cancellationToken);
        }
    }
}
