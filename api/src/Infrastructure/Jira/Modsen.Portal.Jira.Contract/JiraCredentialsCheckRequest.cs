namespace Modsen.Portal.Jira.Contract;

public sealed record JiraCredentialsCheckRequest(Uri JiraBaseUri, string ProjectKey, string Email);
