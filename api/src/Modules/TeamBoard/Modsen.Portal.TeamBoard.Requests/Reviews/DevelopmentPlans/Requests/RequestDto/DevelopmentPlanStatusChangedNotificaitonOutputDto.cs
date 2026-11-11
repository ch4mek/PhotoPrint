namespace Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Requests.RequestDto;

internal class DevelopmentPlanStatusChangedNotificaitonOutputDto
{
    public string? Status { get; set; }
    public string? Message { get; set; }
    public int DevelopmentPlanId { get; set; }
    public int UserId { get; set; }
}
