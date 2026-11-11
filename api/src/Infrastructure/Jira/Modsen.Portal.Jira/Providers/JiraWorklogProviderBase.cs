using System.Globalization;
using System.Net;
using System.Text.Json;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Jira.ApiModels;
using Modsen.Portal.Jira.Contract;
using Modsen.Portal.Jira.Contract.Client;
using Modsen.Portal.Jira.Contract.Exceptions;

namespace Modsen.Portal.Jira.Providers;

public abstract class JiraWorklogProviderBase
{
    protected const int SearchPageSize = 100;
    private const int WorklogPageSize = 1000;
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    public async Task<IReadOnlyCollection<JiraImportedWorklog>> GetWorklogsAsync(
        JiraCredentialsCheckRequest request,
        IJiraApiClient apiClient,
        string? authorEmail,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(apiClient);

        var issues = await SearchIssuesAsync(apiClient, request, startDate, endDate, cancellationToken);
        var result = new List<JiraImportedWorklog>();

        foreach (var issue in issues)
        {
            IReadOnlyCollection<JiraWorklogResponse> worklogs = issue.Fields?.Worklog?.Worklogs ?? [];

            if ((issue.Fields?.Worklog?.Total ?? 0) > worklogs.Count)
            {
                worklogs = await GetIssueWorklogsAsync(apiClient, request, issue.Key, cancellationToken);
            }

            result.AddRange(worklogs
                .Where(worklog => IsStartedInRange(worklog.Started, startDate, endDate))
                .Where(worklog => authorEmail == null || worklog?.Author?.EmailAddress == authorEmail)
                .Select(worklog => new JiraImportedWorklog(
                    issue.Key,
                    issue.Fields?.Summary ?? string.Empty,
                    ParseStartedDate(worklog.Started!),
                    worklog.TimeSpentSeconds)));
        }

        return result;
    }

    protected abstract string ApiVersion { get; }

    protected abstract Task<IReadOnlyCollection<JiraIssueResponse>> SearchIssuesAsync(
        IJiraApiClient apiClient,
        JiraCredentialsCheckRequest request,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken);

    private async Task<IReadOnlyCollection<JiraWorklogResponse>> GetIssueWorklogsAsync(
        IJiraApiClient apiClient,
        JiraCredentialsCheckRequest request,
        string issueKey,
        CancellationToken cancellationToken)
    {
        var worklogs = new List<JiraWorklogResponse>();
        var startAt = 0;
        int total;

        do
        {
            using var response = await apiClient.SendGetAsync(
                JiraUriFactory.BuildIssueWorklogsUri(request.JiraBaseUri, ApiVersion, issueKey, startAt,
                    WorklogPageSize),
                cancellationToken);

            await EnsureSuccessfulImportResponseAsync(response, request.ProjectKey, cancellationToken);

            var page = await DeserializeAsync<JiraWorklogPageResponse>(response, cancellationToken);
            total = page.Total;
            startAt += page.MaxResults == 0 ? WorklogPageSize : page.MaxResults;
            worklogs.AddRange(page.Worklogs);
        } while (startAt < total);

        return worklogs;
    }

    protected static async Task<T> DeserializeAsync<T>(HttpResponseMessage response,
        CancellationToken cancellationToken)
        where T : new()
    {
        ArgumentNullException.ThrowIfNull(response);

        await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonSerializer.DeserializeAsync<T>(contentStream, JsonSerializerOptions, cancellationToken) ??
            new T();
    }

    protected static async Task EnsureSuccessfulImportResponseAsync(
        HttpResponseMessage response,
        string projectKey,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(response);

        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                return;

            case HttpStatusCode.Unauthorized:
                throw new JiraUnauthorizedException(JiraConstants.InvalidPatMessage);

            case HttpStatusCode.Forbidden:
                throw new ActionNotAllowedException($"Access to Jira project {projectKey} is denied");

            case HttpStatusCode.NotFound:
                throw new EntityNotFoundException($"Jira project {projectKey} not found");

            case HttpStatusCode.TooManyRequests:
            case HttpStatusCode.InternalServerError:
            case HttpStatusCode.BadGateway:
            case HttpStatusCode.ServiceUnavailable:
            case HttpStatusCode.GatewayTimeout:
                throw new JiraTemporarilyUnavailableException($"Jira is temporarily unavailable, please try again later");

            default:
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new JiraCredentialsCheckFailedException(
                    $"Jira worklog import failed. StatusCode: {response.StatusCode}. Response: {responseBody}");
        }
    }

    protected static DateOnly ParseStartedDate(string started)
    {
        var startedSpan = started.AsSpan();
        return DateOnly.ParseExact(startedSpan[..10], "yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    protected static bool IsStartedInRange(string? started, DateOnly startDate, DateOnly endDate)
    {
        if (string.IsNullOrWhiteSpace(started))
        {
            return false;
        }

        var startedDate = ParseStartedDate(started);
        return startedDate >= startDate && startedDate <= endDate;
    }
}
