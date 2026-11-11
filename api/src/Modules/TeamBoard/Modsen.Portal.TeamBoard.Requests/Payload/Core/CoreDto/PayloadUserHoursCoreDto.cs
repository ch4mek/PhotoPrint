namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

public class PayloadUserHoursCoreDto
{
    public int MonthOfYear { get; set; }
    public decimal ActualFte { get; set; }
    public decimal PlannedFte { get; set; }
    public decimal PlannedWorkloadInHours { get; set; }
    public decimal TrackedBillableInHours { get; set; }
    public decimal RequiredHours { get; set; }
    public IReadOnlyCollection<PayloadUserProjectHoursCoreDto> ProjectDetails { get; set; } = [];

    public bool HasActivity { get; set; }

    public int TotalUserTime { get; set; }
}
