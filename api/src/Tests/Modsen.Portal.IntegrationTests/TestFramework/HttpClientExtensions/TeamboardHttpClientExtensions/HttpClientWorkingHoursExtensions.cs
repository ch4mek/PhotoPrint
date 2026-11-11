using System.Net.Http.Json;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions
{
    internal static class HttpClientWorkingHoursExtensions
    {
        private static readonly CancellationToken _cancellationToken = CancellationToken.None;

        public static Task<HttpResponseMessage> CreateWorkingHoursPerMonth(this HttpClient client, CreateWorkingHoursPerMonthInput workingHoursToCreate)
        {
            return client.PostAsJsonAsync("api/working-hours-per-months",
                    workingHoursToCreate, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }

        public static Task<HttpResponseMessage> UpdateWorkingHoursPerMonth(this HttpClient client, UpdateWorkingHoursPerMonthInput workingHoursToUpdate)
        {
            return client.PutAsJsonAsync("api/working-hours-per-months",
                    workingHoursToUpdate, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }

        public static Task<HttpResponseMessage> GetWorkingHours(this HttpClient client, int? year)
        {
            var filter = year != null ? "year=" + year : string.Empty;

            return client.GetAsync($"api/working-hours-per-months?{filter}", _cancellationToken);
        }
    }
}
