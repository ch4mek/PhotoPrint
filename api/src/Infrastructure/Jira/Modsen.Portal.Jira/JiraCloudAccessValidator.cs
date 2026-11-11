using System.Net;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Jira.Contract;
using Modsen.Portal.Jira.Contract.Client;
using Modsen.Portal.Jira.Contract.Exceptions;

namespace Modsen.Portal.Jira;

public sealed class JiraCloudAccessValidator : IJiraCloudAccessValidator
{
    public async Task ValidateAsync(
        JiraCredentialsCheckRequest request,
        IJiraApiClient apiClient,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(apiClient);

        await EnsureCloudAuthenticationAsync(request, apiClient, cancellationToken);
        await EnsureProjectExistsAndUserHasAccessAsync(request, apiClient, cancellationToken);
    }

    private static async Task EnsureCloudAuthenticationAsync(
        JiraCredentialsCheckRequest request,
        IJiraApiClient jiraApiClient,
        CancellationToken cancellationToken)
    {
        using var response = await jiraApiClient.SendGetAsync(
            new Uri(request.JiraBaseUri, "/rest/api/3/myself"),
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new JiraUnauthorizedException(JiraConstants.InvalidPatOrEmailMessage);
        }
    }

    private static async Task EnsureProjectExistsAndUserHasAccessAsync(
        JiraCredentialsCheckRequest request,
        IJiraApiClient jiraApiClient,
        CancellationToken cancellationToken)
    {
        using var response = await jiraApiClient.SendGetAsync(
            JiraUriFactory.BuildProjectV3Uri(request.JiraBaseUri, request.ProjectKey),
            cancellationToken);

        switch (response.StatusCode)
        {
            case HttpStatusCode.OK when jiraApiClient.IsJsonResponse(response):
                return;

            case HttpStatusCode.Unauthorized:
                throw new JiraUnauthorizedException(JiraConstants.InvalidPatOrEmailMessage);

            case HttpStatusCode.NotFound:
            case HttpStatusCode.BadRequest:
                throw new InvalidDataAppException($"Jira project {request.ProjectKey} not found");

            case HttpStatusCode.Forbidden:
                throw new ActionNotAllowedException($"Access to Jira project {request.ProjectKey} is denied");

            default:
                throw new JiraCredentialsCheckFailedException();
        }
    }
}
