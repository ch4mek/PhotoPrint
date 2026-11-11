using System.Net.Http.Json;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Requests.RequestDto;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions
{
    internal static class HttpClientPersonalInfoExtensions
    {
        private static readonly CancellationToken _cancellationToken = CancellationToken.None;

        public static Task<HttpResponseMessage> GetUserDetailsAsync(this HttpClient client, int userId)
        {
            return client.GetAsync(
                $"api/personal-info/plan?userId={userId}",
                _cancellationToken);
        }

        public static Task<HttpResponseMessage> UpdateUserPlanAsync(this HttpClient client, int userId, string? plan)
        {
            return client.PatchAsJsonAsync(
                $"api/personal-info/plan?userId={userId}&plan={plan}",
                new UserPlanInput(plan),
                _cancellationToken);
        }

        public static Task<HttpResponseMessage> UpdateUserIsNeedProjectAsync(this HttpClient client, int userId, bool isNeedProject)
        {
            return client.PatchAsync(
                $"api/personal-info/need-project?userId={userId}&isUserNeedProject={isNeedProject}",
                null,
                _cancellationToken);
        }

        public static Task<HttpResponseMessage> GetPersonalInfo(this HttpClient client, int? userId)
        {
            return client.GetAsync(
                $"api/personal-info?userId={userId}",
                _cancellationToken);
        }
        public static Task<HttpResponseMessage> UpdatePersonalInfo(this HttpClient client, UpdatePersonalDataRequestDto input)
        {
            return client.PatchAsJsonAsync("api/personal-info", input, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
        }
        public static Task<HttpResponseMessage> UpdatePersonalInfoComment(this HttpClient client, UpdateTeamBoardUserCommentInput body)
        {
            return client.PatchAsJsonAsync(
                "api/personal-info/comment",
                body,
                _cancellationToken);
        }
    }
}
