namespace Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Requests.RequestDto;

internal record UpdateDevelopmentPlansDto(
    int Id,
    DateOnly? DeadlineDate,
    DateOnly? EndDate,
    string Summary,
    string? Notes);
