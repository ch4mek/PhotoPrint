namespace Modsen.Portal.Jira.Contract;

public sealed class JiraPermissionResponse
{
    public Dictionary<string, JiraPermissionState>? Permissions { get; init; }
}

public sealed class JiraPermissionState
{
    public bool? HavePermission { get; init; }
}
