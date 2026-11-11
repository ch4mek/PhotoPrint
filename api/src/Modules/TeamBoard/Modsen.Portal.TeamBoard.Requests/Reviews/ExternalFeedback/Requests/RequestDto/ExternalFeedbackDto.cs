using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.ExternalFeedback.Requests.RequestDto;

internal record ExternalFeedbackDto(int Id, DateOnly ExternalFeedbackDate, ExternalFeedbackType ExternalFeedbackType,
    string Feedback, string? ActionsTodo, string? Reviewer, int? ConversationId, int ProjectId, int UserId);
