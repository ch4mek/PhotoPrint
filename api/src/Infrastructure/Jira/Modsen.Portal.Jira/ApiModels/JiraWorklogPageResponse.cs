using System.Collections.ObjectModel;

namespace Modsen.Portal.Jira.ApiModels;

public sealed class JiraWorklogPageResponse
{
    public int StartAt { get; set; }
    public int MaxResults { get; set; }
    public int Total { get; set; }
    public Collection<JiraWorklogResponse> Worklogs { get; init; } = [];
}
