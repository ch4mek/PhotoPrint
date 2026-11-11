using Modsen.Portal.Jira.ApiModels;
using Modsen.Portal.Jira.Contract;
using Modsen.Portal.Jira.Contract.Client;
using static Modsen.Portal.Jira.JiraUriFactory;

namespace Modsen.Portal.Jira.Providers;

public sealed class JiraCloudWorklogProvider : JiraWorklogProviderBase, IJiraCloudWorklogProvider
{
    protected override string ApiVersion => "3";

    protected override async Task<IReadOnlyCollection<JiraIssueResponse>> SearchIssuesAsync(
        IJiraApiClient apiClient,
        JiraCredentialsCheckRequest request,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var issues = new List<JiraIssueResponse>();
        string? nextPageToken = null;

        do
        {
            using var response = await apiClient.SendGetAsync(
                JiraUriFactory.BuildWorklogSearchUri(
                    new BuildWorklogSearchModel(
                        request.JiraBaseUri,
                        ApiVersion,
                        request.ProjectKey,
                        request.Email,
                        startDate,
                        endDate),
                    0,
                    SearchPageSize,
                    nextPageToken),
                cancellationToken);

            await EnsureSuccessfulImportResponseAsync(response, request.ProjectKey, cancellationToken);

            var page = await DeserializeAsync<JiraSearchResponse>(response, cancellationToken);
            issues.AddRange(page.Issues);

            nextPageToken = page.NextPageToken;
            if (page.IsLast)
            {
                break;
            }
        }
        while (!string.IsNullOrWhiteSpace(nextPageToken));

        return issues;
    }
}
