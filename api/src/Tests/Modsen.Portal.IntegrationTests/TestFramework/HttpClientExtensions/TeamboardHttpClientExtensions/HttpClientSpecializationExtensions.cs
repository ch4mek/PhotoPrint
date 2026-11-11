using System.Net.Http.Json;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests.RequestDto;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions
{
    internal static class HttpClientSpecializationExtensions
    {
        private static readonly CancellationToken _cancellationToken = CancellationToken.None;
        public static Task<HttpResponseMessage> CreateSpecializationTempplates(this HttpClient client,
        CreateSpecializationTemplateInput specializationToCreate)
        {
            return client.PostAsJsonAsync(
                "api/specialization-templates",
                specializationToCreate,
                IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }

        public static Task<HttpResponseMessage> CreateUserSpecialization(this HttpClient client,
            CreateUserSpecializationInput userSpecializationToCreate)
        {
            return client.PostAsJsonAsync(
                "api/user-specialization",
                userSpecializationToCreate,
                IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }
        public static Task<HttpResponseMessage> DeleteUserSpecialization(this HttpClient client,
            int specializaitonId)
        {
            return client.DeleteAsync($"api/user-specialization/{specializaitonId}",
                _cancellationToken);
        }

        public static Task<HttpResponseMessage> UpdateUserPrimarySpecialization(this HttpClient client,
            UpdatePrimaryUserSpecializationInput updateInput)
        {
            return client.PatchAsJsonAsync(
                "api/user-specialization/primary", updateInput,
                IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }

        public static Task<HttpResponseMessage> UpdateUserSpecialization(this HttpClient client,
            UpdateUserSpecializationsInput updateUserSpecializationsInput)
        {
            return client.PatchAsJsonAsync(
                "api/user-specialization",
                updateUserSpecializationsInput,
                IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }

        public static Task<HttpResponseMessage> GetUserSpecializationLookup(this HttpClient client, int? userId = null, string specialzationTemplateName = null)
        {
            var url = "api/user-specialization/lookup";
            var filter = "?";
            filter += userId != null ? $"userId={userId}" : string.Empty;
            filter += specialzationTemplateName != null ? $"&specializationTemplateName={specialzationTemplateName}" : string.Empty;
            return client.GetAsync(url + filter, _cancellationToken);
        }

        public static Task<HttpResponseMessage> GetUserSkillsSnapshot(this HttpClient client, int? userId = null,
            int? top = null,
            int? skip = null)
        {
            var filter = "?";
            filter += userId != null ? $"userId={userId}" : string.Empty;
            filter += top != null ? $"&Top={top}" : string.Empty;
            filter += skip != null ? $"&Skip={skip}" : string.Empty;

            return client.GetAsync
                ($"api/user-skills-snapshot{filter}", _cancellationToken);
        }

        public static Task<HttpResponseMessage> GetUserSkillsSnapshotByDate(this HttpClient client, int? userId,
            DateOnly date)
        {

            return client.GetAsync
                ($"api/users/{userId}/user-skills-snapshot/{date.ToString("yyyy-MM-dd")}", _cancellationToken);
        }
        public static Task<HttpResponseMessage> GetUserSkillsSnapshotBySnapshotId(this HttpClient client, int snapshotId)
        {
            return client.GetAsync
                ($"api/user-skills-snapshot/{snapshotId}", _cancellationToken);
        }

        public static Task<HttpResponseMessage> GetAssignedUserSpecializations(this HttpClient client, int userId, bool areAssigned)
        {
            return client.GetAsync
                ($"api/users/{userId}/specializations?areAssigned={areAssigned}", _cancellationToken);
        }

        public static Task<HttpResponseMessage> CreateUserSkillSnapshot(this HttpClient client, CreateUserSnapshotInput createUserSnapshotInput)
        {
            return client.PostAsJsonAsync(
                "api/user-skills-snapshot",
                createUserSnapshotInput,
                IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }

        public static Task<HttpResponseMessage> CreateSpecializationTemaplate(this HttpClient client, CreateSpecializationTemplateInput specializationToCreate)
        {
            return client.PostAsJsonAsync(
                "api/specialization-templates",
                specializationToCreate,
                IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }
        public static Task<HttpResponseMessage> UpdateSpecializationTemaplate(this HttpClient client, UpdateSpecializationTemplateInput specializationToCreate)
        {
            return client.PutAsJsonAsync(
                "api/specialization-templates",
                specializationToCreate,
                IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }
        public static Task<HttpResponseMessage> GetSpecializationTemaplate(this HttpClient client, string query)
        {
            return client.GetAsync(
                $"api/specialization-templates{query}", _cancellationToken);
        }
        public static Task<HttpResponseMessage> GetSpecializationTemaplateById(this HttpClient client, int id)
        {
            return client.GetAsync(
                $"api/specialization-templates/{id}", _cancellationToken);
        }
        public static Task<HttpResponseMessage> DeleteSpecializationTemaplate(this HttpClient client, int id)
        {
            return client.DeleteAsync(
                $"api/specialization-templates/{id}", _cancellationToken);
        }
        public static Task<HttpResponseMessage> GetUserSpecializationTemaplate(this HttpClient client, int userId, bool? areAssigned = null)
        {
            var filter = "";
            filter += areAssigned != null ? $"?areAssigned={areAssigned}" : string.Empty;

            return client.GetAsync(
                $"api/users/{userId}/specialization-templates{filter}", _cancellationToken);
        }

        public static Task<SpecializationTemplateWithHardSkillsDto[]> GetUserSpecializationsAsync(
            this HttpClient client, int userId, bool? areAssigned = null)
        {
            var filter = "";
            filter += areAssigned != null ? $"?areAssigned={areAssigned}" : string.Empty;

            return client.GetFromJsonAsync<SpecializationTemplateWithHardSkillsDto[]>(
                $"api/users/{userId}/specializations{filter}",
                IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken)!;
        }
    }
}
