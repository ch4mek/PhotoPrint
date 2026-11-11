using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.Requests.Reviews.ExternalFeedback.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Output;

internal record ExternalFeedbackWithProjectAndConversationOutput(int Id, DateOnly ExternalFeedbackDate,
    ExternalFeedbackType ExternalFeedbackType, string Feedback, string? ActionsTodo, string? Reviewer,
    int? ConversationId, string? ConversationSupervisorFullName, DateOnly? ConversationDate, int ProjectId,
    string ProjectName, int UserId)
{
    public static ExternalFeedbackWithProjectAndConversationOutput Of(ExternalFeedbackWithProjectAndConversationDto result)
    {
        return new ExternalFeedbackWithProjectAndConversationOutput(result.Id, result.ExternalFeedbackDate,
            result.ExternalFeedbackType, result.Feedback, result.ActionsTodo, result.Reviewer,
            result.ConversationId, result.ConversationSupervisorFullName, result.ConversationDate,
            result.ProjectId, result.ProjectName, result.UserId);
    }
};
