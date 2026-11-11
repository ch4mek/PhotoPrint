using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;
using Modsen.Portal.Cv.Requests.Cvs.Requests.RequestDto;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Cv;

internal static class HttpClientCvExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static async Task<HttpResponseMessage> GetPageableCvsAsHttpMessage(
        this HttpClient client,
        Dictionary<string, string?> queryOptions)
    {
        var queryString = string.Join("&", queryOptions.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        return await client.GetAsync(
            new Uri($"api/cvs/search?{queryString}", UriKind.Relative),
            _cancellationToken);
    }

    public static async Task<PageableListOfItems<OdpCvDto>> GetPageableCvs(
        this HttpClient client,
        Dictionary<string, string?> queryOptions,
        JsonSerializerOptions options)
    {
        var queryString = string.Join("&", queryOptions.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        return await client.GetFromJsonAsync<PageableListOfItems<OdpCvDto>>(
            new Uri($"api/cvs/search?{queryString}", UriKind.Relative),
            options,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetCvsLookupAsHttpMessage(this HttpClient client)
    {
        return client.GetAsync(new Uri("api/cvs/lookup", UriKind.Relative), _cancellationToken);
    }

    public static Task<List<CvLookupDto>> GetCvsLookup(this HttpClient client, JsonSerializerOptions options)
    {
        return client.GetFromJsonAsync<List<CvLookupDto>>(
            new Uri("api/cvs/lookup", UriKind.Relative),
            options,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateCvAsHttpMessage(this HttpClient client, CreateCvDto input)
    {
        return client.PostAsJsonAsync(new Uri("api/cvs", UriKind.Relative), input, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<CvCoreDto> CreateCv(this HttpClient client, CreateCvDto input, JsonSerializerOptions options)
    {
        return client.PostAsJsonAsync<CreateCvDto, CvCoreDto>("api/cvs", input, options, _cancellationToken);
    }

    public static Task<HttpResponseMessage> DeleteCv(this HttpClient client, int id)
    {
        return client.DeleteAsync(
            $"api/cvs/{id}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetCv(this HttpClient client, int id)
    {
        return client.GetAsync(
            $"api/cvs/{id}",
            _cancellationToken);
    }
}
