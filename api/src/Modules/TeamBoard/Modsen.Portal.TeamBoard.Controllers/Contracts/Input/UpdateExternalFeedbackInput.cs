using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

internal record UpdateExternalFeedbackInput(int Id, ExternalFeedbackType ExternalFeedbackType, string? ActionsTodo,
    int? ConversationId, string? Reviewer);
