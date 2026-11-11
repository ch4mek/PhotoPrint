namespace Modsen.Portal.Utilities.Controllers.ChatGpt;

public sealed class ChatGptInput
{
    public required string FileUrl { get; set; }
    public required string Greetings { get; set; }
    public required string Description { get; set; }
}
