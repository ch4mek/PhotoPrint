namespace Modsen.Portal.Jira.ApiModels;

public sealed class JiraIssueFieldsResponse
{
    public string? Summary { get; set; }
    public JiraWorklogPageResponse? Worklog { get; set; }
}
