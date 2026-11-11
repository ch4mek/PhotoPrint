using System.Net.Http.Json;
using Modsen.Portal.Cv.Requests.CvProjects.Core.CoreDto;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Cv;

internal static class HttpClientCvProjectsExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static Task<HttpResponseMessage> GetCvProjectsLookupAsHttpMessage(this HttpClient client)
    {
        return client.GetAsync(new Uri("api/cv-projects/lookup", UriKind.Relative), _cancellationToken);
    }

    public static Task<List<CvProjectLookupDto>> GetCvProjectsLookup(this HttpClient client)
    {
        return client.GetFromJsonAsync<List<CvProjectLookupDto>>(
            new Uri("api/cv-projects/lookup", UriKind.Relative),
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }
}
