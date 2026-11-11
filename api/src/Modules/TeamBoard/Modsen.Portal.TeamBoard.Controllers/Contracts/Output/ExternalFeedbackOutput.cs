using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.Requests.Reviews.ExternalFeedback.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Output;

internal record ExternalFeedbackOutput(int Id,
    DateOnly ExternalFeedbackDate,
    ExternalFeedbackType ExternalFeedbackType,
    string Feedback,
    string? ActionsTodo,
    string? Reviewer,
    int? ConversationId,
    int ProjectId,
    int UserId)
{
    public static ExternalFeedbackOutput Of(ExternalFeedbackDto result)
    {
        return new ExternalFeedbackOutput(result.Id, result.ExternalFeedbackDate, result.ExternalFeedbackType,
            result.Feedback, result.ActionsTodo, result.Reviewer, result.ConversationId,
            result.ProjectId, result.UserId);
    }
};
