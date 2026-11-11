using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Tools.PeopleForceDataComposer.API.Adapter.Contract;
using Modsen.Portal.Tools.PeopleForceDataComposer.API.Adapter.PeopleForceApiModels;

namespace Modsen.Portal.Tools.PeopleForceDataComposer.API.Adapter.ApiClient;

internal sealed class PeopleForceApiClient : IPeopleForceApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public PeopleForceApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<JsonDocument> GetByUriAsync(
        string uri,
        int? page,
        CancellationToken cancellationToken)
    {
        var httpClient = GetPeopleForceHttpClient(PeopleForceApiConstants.PeopleForceHttpClientName);

        var requestUri = page.HasValue
            ? uri.Contains('?')
                ? new Uri(uri + $"&page={page}", UriKind.Relative)
                : new Uri(uri + $"?page={page}", UriKind.Relative)
            : new Uri(uri, UriKind.Relative);

        var response = await httpClient.GetAsync(requestUri, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new AppException($"HTTP Error during getting information from PeopleForce API. Reason: {response.ReasonPhrase}");
        }

        var result = response.Content;
        if (result == null)
        {
            throw new AppException("Received empty response from PeopleForce API or unexpected message format");
        }

        var jsonData = await result.ReadAsStringAsync(cancellationToken);
        var jsonDoc = JsonDocument.Parse(jsonData);
        return jsonDoc;
    }

    public async Task<ListOfEmployeesApiModel> GetEmployeesAsync(
        int page, string status,
        CancellationToken cancellationToken)
    {
        var httpClient = GetPeopleForceHttpClient(PeopleForceApiConstants.PeopleForceLeavesHttpClientName);

        status = status.IsNullOrEmpty()
            ? "active"
            : status;

        var url = new Uri($"/api/public/v2/employees?status={status}&page={page}", UriKind.Relative);
        var response = await httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new AppException(
                $"HTTP Error during getting information from PeopleForce API (Get Employees). Reason: {response.ReasonPhrase}");
        }

        var result = await response.Content.ReadFromJsonAsync<ListOfEmployeesApiModel>(cancellationToken);
        if (result == null)
        {
            throw new AppException("Received empty response from PeopleForce API (Get Employees) or unexpected message format");
        }

        return result;
    }

    public async Task<byte[]> GetFileByUriAsync(Uri uri, CancellationToken cancellationToken)
    {
        using var client = GetPeopleForceHttpClient(PeopleForceApiConstants.PeopleForceHttpClientName);
        using var response = await client.GetAsync(uri, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsByteArrayAsync(cancellationToken);
    }

    private HttpClient GetPeopleForceHttpClient(string httpClientName)
    {
        return _httpClientFactory.CreateClient(httpClientName);
    }
}
