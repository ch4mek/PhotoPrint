using System.Text;
using Modsen.Portal.Jira.Contract.Client;

namespace Modsen.Portal.Jira.Client;

internal sealed class JiraApiClientFactory : IJiraApiClientFactory
{
    private readonly IHttpClientFactory _httpClientFactory;

    public JiraApiClientFactory(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public IJiraApiClient CreateBearer(string token)
    {
        var client = _httpClientFactory.CreateClient(JiraConstants.JiraHttpClient);

        return new JiraApiClient(client, "Bearer", token);
    }

    public IJiraApiClient CreateBasic(string email, string token)
    {
        var client = _httpClientFactory.CreateClient(JiraConstants.JiraHttpClient);

        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{email}:{token}"));

        return new JiraApiClient(client, "Basic", credentials);
    }
}
