namespace Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Requests.RequestDto;

internal record CreateDevelopmentPlanDto(int Id, DateOnly StartDate, DateOnly? DeadlineDate, DateOnly? EndDate,
    string Summary, string? Notes, int UserId, int? ConversationId);
