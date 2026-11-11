namespace Modsen.Portal.Jira.Contract.Client;

public interface IJiraApiClient
{
    Task<HttpResponseMessage> SendGetAsync(Uri requestUri, CancellationToken cancellationToken);
    bool IsJsonResponse(HttpResponseMessage response);
}
