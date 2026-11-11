namespace Modsen.Portal.Jira.ApiModels;

public sealed class JiraWorklogResponse
{
    public JiraWorklogAuthorResponse? Author { get; set; }

    public string? Started { get; set; }

    public int TimeSpentSeconds { get; set; }
}

public sealed class JiraWorklogAuthorResponse
{
    public string? Name { get; set; }

    public string? EmailAddress { get; set; }
}
