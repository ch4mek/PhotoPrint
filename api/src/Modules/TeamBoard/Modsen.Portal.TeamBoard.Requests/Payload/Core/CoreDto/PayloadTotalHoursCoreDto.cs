namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

public class PayloadTotalHoursCoreDto
{
    public int MonthOfYear { get; set; }
    public decimal TotalFactBillableInHours { get; set; }
    public decimal TotalPlannedWorkloadInHours { get; set; }
    public decimal TotalActualFte { get; set; }
    public decimal TotalPlannedFte { get; set; }
    public decimal TotalKpi { get; set; }
    public required string Color { get; set; }
    public int TotalTimePerUsers { get; set; }
    public int UsersCount { get; set; }
}
