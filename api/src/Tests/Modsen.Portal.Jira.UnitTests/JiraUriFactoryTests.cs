using AwesomeAssertions;
using static Modsen.Portal.Jira.JiraUriFactory;

namespace Modsen.Portal.Jira.UnitTests;

public class JiraUriFactoryTests
{
    [Fact]
    public void BuildWorklogSearchUriShouldBuildV3UriWithoutNextPageToken()
    {
        // Arrange
        var model = GetBuildWorklogSearchModel();
        var maxResults = 50;

        // Act
        var result = JiraUriFactory.BuildWorklogSearchUri(
            model,
            startAt: 0,
            maxResults: maxResults);

        // Assert
        result.AbsolutePath.Should().Be("/rest/api/3/search/jql");

        result.Query.Should().Contain("jql=");
        result.Query.Should().Contain("fields=summary,worklog");
        result.Query.Should().Contain($"maxResults={maxResults}");
        result.Query.Should().Contain("os_authType=basic");
    }

    [Fact]
    public void BuildWorklogSearchUriWhenApiVersionIs3ShouldUseNextPageToken()
    {
        // Arrange
        var model = GetBuildWorklogSearchModel();

        var startAt = 100;
        var maxResults = 50;
        var nextPageToken = "token with spaces";

        // Act
        var result = JiraUriFactory.BuildWorklogSearchUri(
            model,
            startAt: startAt,
            maxResults: maxResults,
            nextPageToken);

        // Assert
        result.Query.Should().Contain($"nextPageToken={Uri.EscapeDataString(nextPageToken)}");
        result.Query.Should().Contain($"maxResults={maxResults}");
        result.Query.Should().NotContain($"startAt={startAt}");
    }

    [Fact]
    public void BuildWorklogSearchUriWhenApiVersionIsNot3ShouldUseStartAt()
    {
        // Arrange
        var model = GetBuildWorklogSearchModel(apiVersion: "2");
        var startAt = 100;
        var maxResults = 50;

        // Act
        var result = JiraUriFactory.BuildWorklogSearchUri(
            model,
            startAt: startAt,
            maxResults: maxResults,
            nextPageToken: "some-token");

        // Assert
        result.Query.Should().Contain($"startAt={startAt}");
        result.Query.Should().Contain($"maxResults={maxResults}");

        result.Query.Should().NotContain("nextPageToken");
    }

    [Fact]
    public void BuildIssueWorklogsUriShouldBuildCorrectUri()
    {
        // Arrange
        var jiraBaseUri = new Uri("https://jira.example.com/");
        const string apiVersion = "3";
        const string issueKey = "TEST-123";
        const int startAt = 10;
        const int maxResults = 50;

        // Act
        var result = JiraUriFactory.BuildIssueWorklogsUri(
            jiraBaseUri,
            apiVersion,
            issueKey,
            startAt,
            maxResults);

        // Assert
        result.AbsolutePath.Should().Be($"/rest/api/{apiVersion}/issue/{issueKey}/worklog");
        result.Query.Should().Be($"?startAt={startAt}&maxResults={maxResults}&os_authType=basic");
    }

    private static JiraUriFactory.BuildWorklogSearchModel GetBuildWorklogSearchModel(string apiVersion = "3")
    {
        return new JiraUriFactory.BuildWorklogSearchModel
        (
            JiraBaseUri: new Uri("https://jira.example.com/"),
            ApiVersion: apiVersion,
            ProjectKey: "TEST",
            AuthorEmail: "user@example.com",
            StartDate: new DateOnly(2026, 7, 1),
            EndDate: new DateOnly(2026, 7, 28)
        );
    }

}
