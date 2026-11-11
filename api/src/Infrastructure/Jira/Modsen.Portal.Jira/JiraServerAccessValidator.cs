using System.Net;
using Microsoft.Extensions.Logging;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Jira.Contract;
using Modsen.Portal.Jira.Contract.Client;
using Modsen.Portal.Jira.Contract.Exceptions;

namespace Modsen.Portal.Jira;

public sealed class JiraServerAccessValidator : IJiraServerAccessValidator
{
    private readonly ILogger<JiraServerAccessValidator> _logger;

    public JiraServerAccessValidator(ILogger<JiraServerAccessValidator> logger)
    {
        _logger = logger;
    }

    public async Task ValidateAsync(
        JiraCredentialsCheckRequest request,
        HttpResponseMessage searchResponse,
        IJiraApiClient apiClient,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(apiClient);

        switch (searchResponse)
        {
            case { StatusCode: HttpStatusCode.Unauthorized }:
                throw new JiraUnauthorizedException(JiraConstants.InvalidPatMessage);

            case { StatusCode: HttpStatusCode.OK } when apiClient.IsJsonResponse(searchResponse):
                return;

            case { StatusCode: HttpStatusCode.OK }:
                throw new InvalidDataAppException(JiraConstants.InvalidUrlMessage);

            case { StatusCode: HttpStatusCode.BadRequest }:
                await HandleBadRequestAsync(request, searchResponse, apiClient, cancellationToken);
                return;

            case { StatusCode: HttpStatusCode.Forbidden }:
                throw new ActionNotAllowedException($"Access to Jira project {request.ProjectKey} is denied");

            case { StatusCode: HttpStatusCode.TooManyRequests }:
            case { StatusCode: HttpStatusCode.InternalServerError }:
            case { StatusCode: HttpStatusCode.BadGateway }:
            case { StatusCode: HttpStatusCode.ServiceUnavailable }:
            case { StatusCode: HttpStatusCode.GatewayTimeout }:
                throw new JiraTemporarilyUnavailableException($"Jira service is temporarily unavailable, please try again later");

            default:
                throw new JiraCredentialsCheckFailedException();
        }
    }

    private async Task HandleBadRequestAsync(
        JiraCredentialsCheckRequest request,
        HttpResponseMessage response,
        IJiraApiClient jiraApiClient,
        CancellationToken cancellationToken)
    {
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        await EnsureProjectCanBeBrowsedAsync(request, jiraApiClient, cancellationToken);

        _logger.LogError(
            "Malformed Jira JQL query during credentials check for project {ProjectKey}. Response: {ResponseBody}",
            request.ProjectKey, responseBody);

        throw new JiraCredentialsCheckFailedException();
    }

    private async Task EnsureProjectCanBeBrowsedAsync(
        JiraCredentialsCheckRequest request,
        IJiraApiClient jiraApiClient,
        CancellationToken cancellationToken)
    {
        using var permissionResponse = await jiraApiClient.SendGetAsync(
            JiraUriFactory.BuildMyPermissionsUri(request.JiraBaseUri, "2", request.ProjectKey),
            cancellationToken);

        switch (permissionResponse.StatusCode)
        {
            case HttpStatusCode.OK:
                var hasBrowse = await JiraPermissionResponseReader.HasBrowseProjectsPermissionAsync(
                    permissionResponse, cancellationToken);

                if (!hasBrowse)
                {
                    throw new ActionNotAllowedException($"Access to Jira project {request.ProjectKey} is denied");
                }

                return;

            case HttpStatusCode.Unauthorized:
                throw new JiraUnauthorizedException(JiraConstants.InvalidPatMessage);

            case HttpStatusCode.BadRequest:
            case HttpStatusCode.NotFound:
                throw new InvalidDataAppException($"Jira project {request.ProjectKey} not found");

            default:
                await LogPermissionsCheckFailedAsync(permissionResponse, request, cancellationToken);
                throw new JiraCredentialsCheckFailedException();
            }
    }

    private async Task LogPermissionsCheckFailedAsync(
        HttpResponseMessage response,
        JiraCredentialsCheckRequest request,
        CancellationToken cancellationToken)
    {
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogError(
            "Jira permissions check failed for {ProjectKey}. Status: {StatusCode}. Body: {Body}",
            request.ProjectKey, response.StatusCode, body);
    }
}
