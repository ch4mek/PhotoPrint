using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Input
{
    public record CreateOrUpdateInterviewInput(
        int UserId,
        string Specialization,
        string? Client,
        string? JiraTask,
        UserInterviewType? Type,
        UserInterviewStatus? Status,
        DateOnly? InterviewDate,
        TimeOnly? InterviewTimeFromUtc,
        int? SaleManagerId,
        string? ReasonForRefusal,
        string? Notes);
}
