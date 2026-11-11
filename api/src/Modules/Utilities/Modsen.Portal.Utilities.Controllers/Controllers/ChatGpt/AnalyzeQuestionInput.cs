namespace Modsen.Portal.Utilities.Controllers.ChatGpt;

public class AnalyzeQuestionInput
{
    public required string Question { get; set; }
    public string? UserEmail { get; set; }
    public string? Key { get; set; }
    public string? AssistantId { get; set; }
}
