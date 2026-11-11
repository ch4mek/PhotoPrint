namespace Modsen.Portal.Jira.Contract;

public interface IJiraService
{
    Task CheckAccessAsync(
        string jiraUrl,
        string projectKey,
        string? pat,
        string? email,
        CancellationToken cancellationToken);

    Task CheckAccessAsync(
        Uri jiraUrl,
        string projectKey,
        string? pat,
        string? email,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<JiraImportedWorklog>> GetWorklogsAsync(
        string jiraUrl,
        string projectKey,
        string? pat,
        string? authorEmail,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<JiraImportedWorklog>> GetWorklogsAsync(
        Uri jiraUrl,
        string projectKey,
        string? pat,
        string? authorEmail,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken);
}
