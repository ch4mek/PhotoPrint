namespace Modsen.Portal.Jira.ApiModels;

public sealed class JiraIssueResponse
{
    public required string Key { get; set; }
    public JiraIssueFieldsResponse? Fields { get; set; }
}
