using System.Net.Http.Json;
using Modsen.Portal.Cv.Requests.CvClients.Core.CoreDto;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Cv;

internal static class HttpClientCvClientsExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static Task<HttpResponseMessage> GetCvClientsLookupAsHttpMessage(this HttpClient client)
    {
        return client.GetAsync(new Uri("api/cv-clients/lookup", UriKind.Relative), _cancellationToken);
    }

    public static Task<List<CvClientLookupDto>> GetCvClientsLookup(this HttpClient client)
    {
        return client.GetFromJsonAsync<List<CvClientLookupDto>>(
            new Uri("api/cv-clients/lookup", UriKind.Relative),
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }
}
