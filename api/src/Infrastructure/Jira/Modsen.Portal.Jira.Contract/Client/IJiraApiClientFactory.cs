namespace Modsen.Portal.Jira.Contract.Client;

public interface IJiraApiClientFactory
{
    IJiraApiClient CreateBearer(string token);
    IJiraApiClient CreateBasic(string email, string token);
}
