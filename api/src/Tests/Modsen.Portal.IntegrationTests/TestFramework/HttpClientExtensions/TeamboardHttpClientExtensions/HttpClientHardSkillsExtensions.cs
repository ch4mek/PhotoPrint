using System.Net.Http.Json;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions
{
    internal static class HttpClientHardSkillsExtensions
    {
        private static readonly CancellationToken _cancellationToken = CancellationToken.None;

        public static Task<HttpResponseMessage> CreateHardSkills(this HttpClient client, CreateHardSkillInput createHardSkillInput)
        {
            return client.PostAsJsonAsync(
                "api/hard-skills", createHardSkillInput, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }

        public static Task<HttpResponseMessage> UpdateHardSkills(this HttpClient client, UpdateHardSkillInput updateHardSkillInput)
        {
            return client.PutAsJsonAsync(
                "api/hard-skills",
                updateHardSkillInput,
                _cancellationToken);
        }

        public static Task<HttpResponseMessage> DeleteHardSkills(this HttpClient client, int id)
        {
            return client.DeleteAsync($"api/hard-skills/{id}", _cancellationToken);
        }

        public static Task<HttpResponseMessage> GetHardSkills(this HttpClient client, string query)
        {
            return client.GetAsync(
                $"api/hard-skills{query}",
                _cancellationToken);
        }

        public static Task<HttpResponseMessage> GetHardSkillsLookup(this HttpClient client)
        {
            return client.GetAsync(
                "api/hard-skills/lookup",
                _cancellationToken);
        }

        public static Task<HttpResponseMessage> GetHardSkills(this HttpClient client, int id)
        {
            return client.GetAsync(
                $"api/hard-skills/{id}",
                _cancellationToken);
        }
    }
}
