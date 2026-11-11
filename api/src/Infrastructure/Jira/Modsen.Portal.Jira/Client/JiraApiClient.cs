using System.Net.Http.Headers;
using Modsen.Portal.Jira.Contract.Client;
using Modsen.Portal.Jira.Contract.Exceptions;

namespace Modsen.Portal.Jira.Client;

public sealed class JiraApiClient : IJiraApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _authScheme;
    private readonly string _authParameter;

    public JiraApiClient(HttpClient httpClient, string authScheme, string authParameter)
    {
        _httpClient = httpClient;
        _authScheme = authScheme;
        _authParameter = authParameter;
    }

    public async Task<HttpResponseMessage> SendGetAsync(
        Uri requestUri,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

        request.Headers.Authorization = new AuthenticationHeaderValue(_authScheme, _authParameter);

        try
        {
            return await _httpClient.SendAsync(request, cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            throw new JiraTemporarilyUnavailableException(JiraConstants.UrlNotFoundMessage);
        }
    }

    public bool IsJsonResponse(HttpResponseMessage response)
    {
        ArgumentNullException.ThrowIfNull(response);

        return string.Equals(
            response.Content.Headers.ContentType?.MediaType,
            "application/json",
            StringComparison.OrdinalIgnoreCase);
    }
}
