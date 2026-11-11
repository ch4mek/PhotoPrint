namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

public class PayloadUserProjectHoursCoreDto
{
    public int ProjectId { get; set; }
    public decimal ActualFte { get; set; }
    public decimal PlannedFte { get; set; }
    public decimal TrackedBillableInHours { get; set; }
    public decimal RequiredHours { get; set; }
}
