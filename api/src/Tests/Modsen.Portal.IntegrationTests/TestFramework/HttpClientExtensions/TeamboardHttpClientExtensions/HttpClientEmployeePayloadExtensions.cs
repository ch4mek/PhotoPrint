using System.Net.Http.Json;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions
{
    internal static class HttpClientEmployeePayloadExtensions
    {
        private static readonly CancellationToken _cancellationToken = CancellationToken.None;

        public static Task<HttpResponseMessage> CreateEmployeePayloadCalendar(this HttpClient client, int year, GetEmployeePayloadInput employeePayload)
        {
            return client.PostAsJsonAsync($"/api/calendar/{year}/employees-payload",
                employeePayload, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }
        public static Task<HttpResponseMessage> UpdateEmployeePayloadCalendar(this HttpClient client, int? userId, int year, int month, UpdateUserPayloadFteInput payloadFteInput)
        {
            return client.PutAsJsonAsync($"api/calendar/{year}/month/{month}/users/{userId}",
                payloadFteInput, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }

        public static Task<HttpResponseMessage> UpdatePayloadUserDetails(this HttpClient client, int? userId, UpdateUserPayloadDetailsInput input)
        {
            return client.PutAsJsonAsync($"/api/users/{userId}/payload-details",
                input, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }
    }
}
