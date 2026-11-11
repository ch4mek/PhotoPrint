using System.Net.Http.Json;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Requests.RequestDto;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions
{
    internal static class HttpClientSalaryExtensions
    {
        private static readonly CancellationToken _cancellationToken = CancellationToken.None;

        public static Task<HttpResponseMessage> GetSalary(this HttpClient client, int userId)
        {
            return client.GetAsync(
                $"api/Users/{userId}/salaries?$orderby=salaryFrom%20desc&$top=50&$skip=0", _cancellationToken);
        }
        public static Task<HttpResponseMessage> CreateUserSalary(this HttpClient client, CreateSalaryDto createUserSalary)
        {
            return client.PostAsJsonAsync(
                "api/user-salary", createUserSalary, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }

        public static Task<HttpResponseMessage> UpdateUserSalary(this HttpClient client, UpdateSalaryDto contentForUpdating)
        {
            return client.PatchAsJsonAsync("api/user-salary", contentForUpdating, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }
        public static Task<HttpResponseMessage> CreateUserSalaryDetailsList(this HttpClient client, List<CreateSalaryDto> createUserSalary)
        {
            return client.PostAsJsonAsync(
                "api/user-salary/salaries", createUserSalary, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }
        public static Task<HttpResponseMessage> UpdateUserSalaryDetailsList(this HttpClient client, List<UpdateSalaryDto> contentForUpdating)
        {
            return client.PatchAsJsonAsync("api/user-salary/salaries", contentForUpdating, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }
        public static Task<HttpResponseMessage> GetUserSalaryDetailsById(this HttpClient client, int salaryId)
        {
            return client.GetAsync($"api/user-salary/{salaryId}", _cancellationToken);
        }
    }
}
