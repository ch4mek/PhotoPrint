using Modsen.Portal.Jira.Contract;
using Modsen.Portal.Jira.Contract.Client;

namespace Modsen.Portal.Jira.Providers;

public interface IJiraServerWorklogProvider
{
    Task<IReadOnlyCollection<JiraImportedWorklog>> GetWorklogsAsync(
        JiraCredentialsCheckRequest request,
        IJiraApiClient apiClient,
        string? authorEmail,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken);
}

public interface IJiraCloudWorklogProvider
{
    Task<IReadOnlyCollection<JiraImportedWorklog>> GetWorklogsAsync(
        JiraCredentialsCheckRequest request,
        IJiraApiClient apiClient,
        string? authorEmail,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken);
}
