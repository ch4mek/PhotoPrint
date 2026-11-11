using Modsen.Portal.Jira.ApiModels;
using Modsen.Portal.Jira.Contract;
using Modsen.Portal.Jira.Contract.Client;
using static Modsen.Portal.Jira.JiraUriFactory;

namespace Modsen.Portal.Jira.Providers;

public sealed class JiraServerWorklogProvider : JiraWorklogProviderBase, IJiraServerWorklogProvider
{
    protected override string ApiVersion => "2";

    protected override async Task<IReadOnlyCollection<JiraIssueResponse>> SearchIssuesAsync(
        IJiraApiClient apiClient,
        JiraCredentialsCheckRequest request,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var issues = new List<JiraIssueResponse>();
        var startAt = 0;
        int total;

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
                    startAt,
                    SearchPageSize),
                cancellationToken);

            await EnsureSuccessfulImportResponseAsync(response, request.ProjectKey, cancellationToken);

            var page = await DeserializeAsync<JiraSearchResponse>(response, cancellationToken);
            total = page.Total;
            startAt += page.MaxResults == 0 ? SearchPageSize : page.MaxResults;
            issues.AddRange(page.Issues);
        }
        while (startAt < total);

        return issues;
    }
}
