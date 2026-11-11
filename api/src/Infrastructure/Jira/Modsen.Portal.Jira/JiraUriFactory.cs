using Modsen.Portal.Core.Exceptions;

namespace Modsen.Portal.Jira;

internal static class JiraUriFactory
{
    private const string RootPath = "/";
    private const string PathDelimiter = "/";

    public static Uri NormalizeBaseUri(string jiraUrl)
    {
        var normalizedUrl = jiraUrl.Trim();

        if (!normalizedUrl.Contains("://", StringComparison.Ordinal))
        {
            normalizedUrl = $"https://{normalizedUrl}";
        }

        if (!Uri.TryCreate(normalizedUrl.TrimEnd('/') + PathDelimiter, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new EntityNotFoundException(JiraConstants.UrlNotFoundMessage);
        }

        if (uri.AbsolutePath != RootPath)
        {
            throw new InvalidDataAppException(JiraConstants.InvalidUrlMessage);
        }

        return uri;
    }

    public static Uri BuildSearchV2Uri(Uri baseUri, string projectKey)
    {
        return new Uri(baseUri, $"/rest/api/2/search?jql=project={projectKey}");
    }

    public static Uri BuildProjectV3Uri(Uri baseUri, string projectKey)
    {
        return new Uri(baseUri, $"/rest/api/3/project/{projectKey}");
    }

    public static Uri BuildWorklogSearchUri(
        BuildWorklogSearchModel model,
        int startAt,
        int maxResults,
        string? nextPageToken = null)
    {
        var searchPath = model.ApiVersion == "3" ? "search/jql" : "search";
        var endExclusive = model.EndDate.AddDays(1);
        var jql = $"project=\"{model.ProjectKey}\" AND worklogAuthor=\"{model.AuthorEmail}\" " +
            $"AND worklogDate>=\"{model.StartDate:yyyy-MM-dd}\" AND worklogDate<\"{endExclusive:yyyy-MM-dd}\"";
        var jqlEncoded = Uri.EscapeDataString(jql);

        string pagingQuery;
        if(model.ApiVersion == "3")
        {
            pagingQuery = string.IsNullOrWhiteSpace(nextPageToken)
                ? $"&maxResults={maxResults}"
                : $"&nextPageToken={Uri.EscapeDataString(nextPageToken)}&maxResults={maxResults}";
        }
        else
        {
            pagingQuery = $"&startAt={startAt}&maxResults={maxResults}";
        }

        var relativeUri =
            $"rest/api/{model.ApiVersion}/{searchPath}?jql={jqlEncoded}&fields=summary,worklog" +
            $"{pagingQuery}&os_authType=basic";

        return new Uri(model.JiraBaseUri, relativeUri);
    }

    public static Uri BuildIssueWorklogsUri(Uri jiraBaseUri, string apiVersion, string issueKey, int startAt, int maxResults)
    {
        return new Uri(
            jiraBaseUri,
            $"rest/api/{apiVersion}/issue/{Uri.EscapeDataString(issueKey)}/worklog" +
            $"?startAt={startAt}&maxResults={maxResults}&os_authType=basic");
    }

    public static Uri BuildMyPermissionsUri(Uri jiraBaseUri, string apiVersion, string projectKey)
    {
        return new Uri(
            jiraBaseUri,
            $"rest/api/{apiVersion}/mypermissions?projectKey={Uri.EscapeDataString(projectKey)}" +
            $"&permissions={JiraConstants.BrowseProjectsPermission}&os_authType=basic");
    }

    internal record BuildWorklogSearchModel(
        Uri JiraBaseUri,
        string ApiVersion,
        string ProjectKey,
        string AuthorEmail,
        DateOnly StartDate,
        DateOnly EndDate);
}
