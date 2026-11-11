using Modsen.Portal.Jira.Contract.Client;

namespace Modsen.Portal.Jira.Contract;

public interface IJiraCloudAccessValidator
{
    Task ValidateAsync(JiraCredentialsCheckRequest request, IJiraApiClient apiClient, CancellationToken cancellationToken);
}
