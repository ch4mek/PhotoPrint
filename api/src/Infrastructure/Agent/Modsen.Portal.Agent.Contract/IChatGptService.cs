namespace Modsen.Portal.Agent.Contract;

public interface IChatGptService
{
    Task<IEnumerable<string>?> Compare(string fileUrl, string greetings, string description, string jiraToken);
    Task<string?> AnalyzeQuestion(string question, string? userEmail, string? key, string? assistantId, CancellationToken cancellationToken);
    Task<string> CreateThreadForUser(CancellationToken cancellationToken);
    string GetAssistantId();
}
