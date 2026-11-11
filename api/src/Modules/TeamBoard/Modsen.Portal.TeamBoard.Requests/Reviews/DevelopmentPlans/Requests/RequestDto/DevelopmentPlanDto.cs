namespace Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Requests.RequestDto;

internal record DevelopmentPlanDto(int Id, DateOnly StartDate, DateOnly? DeadlineDate, DateOnly? EndDate,
    string Summary, string? Notes, int UserId);
