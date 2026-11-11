using Modsen.Portal.Jira.Contract.Client;

namespace Modsen.Portal.Jira.Contract;

public interface IJiraServerAccessValidator
{
    Task ValidateAsync(
        JiraCredentialsCheckRequest request,
        HttpResponseMessage searchResponse,
        IJiraApiClient apiClient,
        CancellationToken cancellationToken);
}
