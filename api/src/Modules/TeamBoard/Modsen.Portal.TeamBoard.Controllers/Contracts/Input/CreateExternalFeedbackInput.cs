using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

internal record CreateExternalFeedbackInput(DateOnly ExternalFeedbackDate, ExternalFeedbackType ExternalFeedbackType,
    string Feedback, string? ActionsTodo, string? Reviewer, int? ConversationId, int ProjectId, int UserId);
