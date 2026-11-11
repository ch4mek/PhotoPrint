namespace Modsen.Portal.Core.Identity;

public static class JiraClaimTypeNames
{
    public const string GroupId = "JIRA";
    public static readonly ClaimType ViewJiraImport = new("ViewJiraImport", 100);
}
