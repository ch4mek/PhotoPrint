using System.Net.Http.Json;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions
{
    internal static class HttpClientSoftSkillsExtensions
    {
        private static readonly CancellationToken _cancellationToken = CancellationToken.None;

        public static Task<HttpResponseMessage> GetSoftSkills(this HttpClient client, string query = null)
        {
            return client.GetAsync(
               $"api/soft-skills{query}",
               _cancellationToken);
        }

        public static Task<HttpResponseMessage> GetSoftSkills(this HttpClient client, string query, StringContent content)
        {
            return client.PostAsync(
               $"api/soft-skills{query}",
               content,
               _cancellationToken);
        }

        public static Task<HttpResponseMessage> GetSoftSkillsById(this HttpClient client, int Id)
        {
            return client.GetAsync(
               $"api/soft-skills/{Id}",
               _cancellationToken);
        }

        public static Task<HttpResponseMessage> GetUsersSoftSkillsAsHttpMessage(this HttpClient client, int? userId, string query = null)
        {
            return client.GetAsync(
               $"api/users/{userId}/soft-skills{query}",
               _cancellationToken);
        }

        public static Task<PageableListOfItems<OdpUserSoftSkill>> GetUsersSoftSkills(this HttpClient client, int? userId, string query = null)
        {
            return client.GetFromJsonAsync<PageableListOfItems<OdpUserSoftSkill>>(
                $"api/users/{userId}/soft-skills{query}",
                _cancellationToken);
        }

        public static Task<HttpResponseMessage> CreateUsersSoftSkills(this HttpClient client, int? userId, CreateUserSoftSkillsSnapshotInput updateUserSoftSkillInput)
        {
            return client.PostAsJsonAsync(
                $"api/users/{userId}/soft-skills/",
                updateUserSoftSkillInput,
                _cancellationToken);
        }

        public static Task<HttpResponseMessage> CreateSoftSkills(this HttpClient client, CreateSoftSkillInput softSkillToCreate)
        {
            return client.PostAsJsonAsync(
                "api/soft-skills",
                softSkillToCreate,
                _cancellationToken);
        }
        public static Task<HttpResponseMessage> UpdateSoftSkills(this HttpClient client, UpdateSoftSkillInput updateSoftSkillInput)
        {
            return client.PutAsJsonAsync(
                "api/soft-skills",
                updateSoftSkillInput,
                _cancellationToken);
        }

        public static Task<HttpResponseMessage> DeleteSoftSkills(this HttpClient client, int id)
        {
            return client.DeleteAsync(
                $"api/soft-skills/{id}",
                _cancellationToken);
        }

        public static Task<HttpResponseMessage> UpdateUsersSoftSkills(this HttpClient client, int? snapshotId, UpdateUserSoftSkillsSnapshotInput updateUserSoftSkillInput)
        {
            return client.PutAsJsonAsync(
                $"api/users/soft-skills/{snapshotId}",
                updateUserSoftSkillInput,
                _cancellationToken);
        }
    }
}
