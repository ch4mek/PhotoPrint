using Modsen.Portal.Agent.Contract.Models;

namespace Modsen.Portal.Agent.Contract;

public interface ICopilotService
{
    Task<(ConversationSession Session, IReadOnlyList<string> Messages)> StartNewConversationAsync(
        Dictionary<string, object?>? userData, int userId,
        CancellationToken cancellationToken);

    Task<RefreshOrResumeConversationDto> RefreshOrResumeConversationAsync(
       ConversationSession? existingSession,
       Dictionary<string, object?>? userData, int userId,
       CancellationToken cancellationToken);

    Task<IReadOnlyList<string>> SendMessageAsync(
        ConversationSession session, int userId,
        string text,
        CancellationToken cancellationToken);
}

public sealed record RefreshOrResumeConversationDto(
    ConversationSession ConversationSession,
    bool IsNewConversation,
    IReadOnlyList<string>? messages = null);
