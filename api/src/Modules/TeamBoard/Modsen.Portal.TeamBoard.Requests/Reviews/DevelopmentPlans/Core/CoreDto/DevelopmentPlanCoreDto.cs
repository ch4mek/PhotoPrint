using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Core.CoreDto
{
    internal record DevelopmentPlanCoreDto(
        int Id,
        DateOnly StartDate,
        DateOnly? DeadlineDate,
        DateOnly? EndDate,
        string Summary,
        string? Notes,
        int UserId,
        string? UserPosition,
        DevelopmentPlanStatusType Status);
}
