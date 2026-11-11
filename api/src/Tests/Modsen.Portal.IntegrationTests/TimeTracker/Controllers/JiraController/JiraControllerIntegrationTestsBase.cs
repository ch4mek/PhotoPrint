using Modsen.Portal.Jira.Contract;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Moq;

namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.JiraController;

public abstract class JiraControllerIntegrationTestsBase
{
    protected const string JiraUrl = "https://jira.example.com";
    protected const string JiraProjectKey = "MMS";
    protected const string Pat = "jira-pat";

    protected readonly JiraIntegrationTestFactory Factory;
    protected readonly Mock<IJiraService> JiraServiceMock;

    protected JiraControllerIntegrationTestsBase(JiraIntegrationTestFactory factory)
    {
        Factory = factory;
        JiraServiceMock = factory.JiraServiceMock;
        JiraServiceMock.Reset();
        SetupSuccessfulJiraCredentialsCheck();
    }

    public static IEnumerable<object?[]> GetInvalidCreateParams()
    {
        yield return [BuildInput(url: "")];
        yield return [BuildInput(url: "ftp://jira.example.com")];
        yield return [BuildInput(pat: "")];
        yield return [BuildInput(jiraProjectTag: "")];
        yield return [BuildInput(projectId: 0)];
        yield return [BuildInput(userId: 0)];
    }

    public static IEnumerable<object?[]> GetInvalidUpdateParams()
    {
        yield return [BuildUpdateInput(url: "")];
        yield return [BuildUpdateInput(url: "ftp://jira.example.com")];
        yield return [BuildUpdateInput(jiraProjectTag: "")];
        yield return [BuildUpdateInput(projectId: 0)];
        yield return [BuildUpdateInput(userId: 0)];
    }

    protected static CreateJiraSettingInput BuildInput(
        string url = JiraUrl,
        string pat = Pat,
        string jiraProjectTag = JiraProjectKey,
        int? projectId = null,
        int? userId = null,
        string? email = null)
    {
        return new CreateJiraSettingInput
        {
            Url = url,
            Pat = pat,
            JiraProjectTag = jiraProjectTag,
            ProjectId = projectId ?? 1,
            UserId = userId ?? 1,
            Email = email ?? "test@email.com",
        };
    }

    protected CreateJiraSettingInput CreateValidInput(
        string url = JiraUrl,
        string pat = Pat,
        string jiraProjectTag = JiraProjectKey,
        string email = "test@email.com")
    {
        var projectTag = jiraProjectTag == JiraProjectKey
            ? $"{jiraProjectTag}{Guid.NewGuid():N}"[..12]
            : jiraProjectTag;

        return BuildInput(
            url,
            pat,
            projectTag,
            Factory.ModsenProjectId!.Value,
            Factory.EmployeeUserId!.Value,
            email);
    }

    protected static UpdateJiraSettingInput BuildUpdateInput(
        string url = JiraUrl,
        string? pat = Pat,
        string jiraProjectTag = JiraProjectKey,
        int? projectId = null,
        int? userId = null,
        string? email = null)
    {
        return new UpdateJiraSettingInput
        {
            Url = url,
            Pat = pat,
            JiraProjectTag = jiraProjectTag,
            ProjectId = projectId ?? 1,
            UserId = userId ?? 1,
            Email = email ?? "test@email.com",
        };
    }

    protected UpdateJiraSettingInput CreateValidUpdateInput(
        string url = JiraUrl,
        string? pat = Pat,
        string jiraProjectTag = JiraProjectKey,
        string email = "test@email.com")
    {
        return BuildUpdateInput(
            url,
            pat,
            jiraProjectTag,
            Factory.ModsenProjectId!.Value,
            Factory.EmployeeUserId!.Value,
            email);
    }

    private void SetupSuccessfulJiraCredentialsCheck()
    {
        JiraServiceMock
            .Setup(x => x.CheckAccessAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }
}
