using System.Net.Http.Json;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;

internal static class HttpClientLabBoardExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static Task<HttpResponseMessage> GetPageableInterns(this HttpClient client, string query)
    {
        return client.GetAsync($"api/labboard{query}", _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetLabBoardInternLookup(this HttpClient client, string query)
    {
        return client.GetAsync($"api/labboard/lookup{query}", _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetInternship(this HttpClient client, int userId)
    {
        return client.GetAsync($"api/labboard/{userId}/internship", _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetInternshipSpecializationsWithHardSkills(this HttpClient client)
    {
        return client.GetAsync($"api/labboard/internship/specializations", _cancellationToken);
    }

    public static Task<HttpResponseMessage> EditInternship(this HttpClient client, int userId, EditInternshipInput editInternshipInput)
    {
        return client.PutAsJsonAsync($"api/labboard/{userId}/internship", editInternshipInput, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }
}
