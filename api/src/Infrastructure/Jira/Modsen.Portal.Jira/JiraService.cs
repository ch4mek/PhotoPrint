using System.Net;
using Modsen.Portal.Jira.Contract;
using Modsen.Portal.Jira.Contract.Client;
using Modsen.Portal.Jira.Providers;

namespace Modsen.Portal.Jira;

public sealed class JiraService : IJiraService
{
    private readonly IJiraApiClientFactory _apiClientFactory;
    private readonly IJiraServerAccessValidator _serverValidator;
    private readonly IJiraCloudAccessValidator _cloudValidator;
    private readonly IJiraServerWorklogProvider _serverWorklogProvider;
    private readonly IJiraCloudWorklogProvider _cloudWorklogProvider;

    public JiraService(
        IJiraApiClientFactory apiClientFactory,
        IJiraServerAccessValidator serverValidator,
        IJiraCloudAccessValidator cloudValidator,
        IJiraServerWorklogProvider serverWorklogProvider,
        IJiraCloudWorklogProvider cloudWorklogProvider)
    {
        _apiClientFactory = apiClientFactory;
        _serverValidator = serverValidator;
        _cloudValidator = cloudValidator;
        _serverWorklogProvider = serverWorklogProvider;
        _cloudWorklogProvider = cloudWorklogProvider;
    }

    public async Task CheckAccessAsync(
        string jiraUrl,
        string projectKey,
        string? pat,
        string? email,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jiraUrl);
        var baseUri = JiraUriFactory.NormalizeBaseUri(jiraUrl);

        await CheckAccessAsync(baseUri, projectKey, pat, email, cancellationToken);
    }

    public async Task CheckAccessAsync(
        Uri jiraUrl,
        string projectKey,
        string? pat,
        string? email,
    CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(jiraUrl);

        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        ArgumentException.ThrowIfNullOrWhiteSpace(pat);
        
        var request = new JiraCredentialsCheckRequest(jiraUrl, projectKey, email);

        var serverClient = _apiClientFactory.CreateBearer(pat);

        using var searchResponse = await serverClient.SendGetAsync(
            JiraUriFactory.BuildSearchV2Uri(request.JiraBaseUri, request.ProjectKey),
            cancellationToken);

        if (!ShouldFallbackToCloud(searchResponse))
        {
            await _serverValidator.ValidateAsync(request, searchResponse, serverClient, cancellationToken);
            return;
        }

        var cloudClient = _apiClientFactory.CreateBasic(email, pat);

        await _cloudValidator.ValidateAsync(request, cloudClient, cancellationToken);
    }

    public async Task<IReadOnlyCollection<JiraImportedWorklog>> GetWorklogsAsync(
        string jiraUrl,
        string projectKey,
        string? pat,
        string? authorEmail,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jiraUrl);
        var baseUri = JiraUriFactory.NormalizeBaseUri(jiraUrl);

        return await GetWorklogsAsync(baseUri, projectKey, pat, authorEmail, startDate, endDate, cancellationToken);
    }

    public async Task<IReadOnlyCollection<JiraImportedWorklog>> GetWorklogsAsync(
        Uri jiraUrl,
        string projectKey,
        string? pat,
        string? authorEmail,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(jiraUrl);

        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(authorEmail);

        ArgumentException.ThrowIfNullOrWhiteSpace(pat);

        var request = new JiraCredentialsCheckRequest(
            jiraUrl,
            projectKey, authorEmail);

        var serverClient = _apiClientFactory.CreateBearer(pat);

        using var searchResponse = await serverClient.SendGetAsync(
            JiraUriFactory.BuildSearchV2Uri(request.JiraBaseUri, request.ProjectKey),
            cancellationToken);

        if (!ShouldFallbackToCloud(searchResponse))
        {
            return await _serverWorklogProvider.GetWorklogsAsync(request, serverClient, authorEmail, startDate, endDate, cancellationToken);
        }

        var cloudClient = _apiClientFactory.CreateBasic(authorEmail, pat);

        return await _cloudWorklogProvider.GetWorklogsAsync(request, cloudClient, authorEmail, startDate, endDate, cancellationToken);
    }

    private static bool ShouldFallbackToCloud(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.Gone)
        {
            return true;
        }

        if (response.Headers.Contains("Atl-Request-Id") ||
            response.Headers.Contains("Atl-Traceid"))
        {
            return true;
        }

        var isAtlassianEdge = response.Headers.Server
            .Any(s => s.Product != null &&
                s.Product.Name.Contains("AtlassianEdge", StringComparison.OrdinalIgnoreCase));

        return isAtlassianEdge;
    }
}
