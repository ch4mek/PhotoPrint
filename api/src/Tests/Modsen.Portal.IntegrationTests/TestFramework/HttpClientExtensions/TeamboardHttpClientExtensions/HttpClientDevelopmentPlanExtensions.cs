using System.Net.Http.Json;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions
{
    internal static class HttpClientDevelopmentPlanExtensions
    {
        private static readonly CancellationToken _cancellationToken = CancellationToken.None;

        public static Task<HttpResponseMessage> GetDevelopmentPlans(this HttpClient client, string query)
        {
            return client.GetAsync($"api/development-plans{query}", _cancellationToken);
        }
        public static Task<HttpResponseMessage> GetDevelopmentPlans(this HttpClient client, int id)
        {
            return client.GetAsync($"api/development-plans/{id}");
        }
        public static Task<HttpResponseMessage> CreateDevelopmentPlans(this HttpClient client, CreateDevelopmentPlanInput developmentPlanToCreate)
        {
            return client.PostAsJsonAsync("api/development-plans", developmentPlanToCreate,
                IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }
        public static Task<HttpResponseMessage> UpdateDevelopmentPlans(this HttpClient client, UpdateDevelopmentPlanInput developmentPlanToUpdate)
        {
            return client.PutAsJsonAsync("api/development-plans/", developmentPlanToUpdate, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }
        public static Task<HttpResponseMessage> UpdateDevelopmentPlanStatus(this HttpClient client, UpdateDevelopmentPlanStatusInput developmentPlanToUpdate)
        {
            return client.PatchAsJsonAsync("api/development-plans/development-plans/status", developmentPlanToUpdate, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }
        public static Task<HttpResponseMessage> DeleteDevelopmentPlan(this HttpClient client, int id)
        {
            return client.DeleteAsync($"api/development-plans/{id}", _cancellationToken);
        }
        public static Task<HttpResponseMessage> CreateDevelopmentPlanList(this HttpClient client, List<CreateDevelopmentPlanInput> developmentPlansToCreate)
        {
            return client.PostAsJsonAsync("api/development-plans/development-plans", developmentPlansToCreate, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }
        public static Task<HttpResponseMessage> UpdateDevelopmentPlanList(this HttpClient client, List<UpdateDevelopmentPlanInput> developmentPlansToUpdate)
        {
            return client.PatchAsJsonAsync("api/development-plans/development-plans", developmentPlansToUpdate, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }
        public static Task<HttpResponseMessage> GetSupervisorsByUserId(this HttpClient client, int id)
        {
            return client.GetAsync($"api/Conversations/supervisors?userId={id}");
        }
    }
}
