using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.ExternalFeedback.Requests.RequestDto;

internal record ExternalFeedbackWithProjectAndConversationDto(int Id, DateOnly ExternalFeedbackDate,
    ExternalFeedbackType ExternalFeedbackType, string Feedback, string? ActionsTodo, string? Reviewer, int? ConversationId,
    string? ConversationSupervisorFullName, DateOnly? ConversationDate, int ProjectId, string ProjectName, int UserId);
