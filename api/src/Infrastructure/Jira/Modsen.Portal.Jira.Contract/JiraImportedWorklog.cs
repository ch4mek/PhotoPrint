namespace Modsen.Portal.Jira.Contract;

public sealed record JiraImportedWorklog(
    string IssueKey,
    string IssueSummary,
    DateOnly StartedDate,
    int TimeSpentSeconds);
