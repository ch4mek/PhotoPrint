using System.Net.Http.Json;
using Modsen.Portal.Cv.Requests.CvUsers.Core.CoreDto;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Cv;

internal static class HttpClientCvUsersExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static Task<HttpResponseMessage> GetCvUsersLookupAsHttpMessage(this HttpClient client)
    {
        return client.GetAsync(new Uri("api/cv-users/lookup", UriKind.Relative), _cancellationToken);
    }

    public static Task<List<CvUserLookupDto>> GetCvUsersLookup(this HttpClient client)
    {
        return client.GetFromJsonAsync<List<CvUserLookupDto>>(
            new Uri("api/cv-users/lookup", UriKind.Relative),
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }
}
