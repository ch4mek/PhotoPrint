namespace Modsen.Portal.Jira.ApiModels;

internal sealed class JiraSearchResponse
{
    public int StartAt { get; set; }
    public int MaxResults { get; set; }
    public int Total { get; set; }
    public bool IsLast { get; set; }
    public string? NextPageToken { get; set; }
    public List<JiraIssueResponse> Issues { get; set; } = [];
}
