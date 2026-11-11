namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

internal sealed class KpiTimesDto
{
    public int ActiveHeadsCount { get; set; }
    public decimal TrackedBillableTime { get; set; }
    public int TotalUserTime { get; set; }
    public int HeadNotWorkingCount { get; set; }
}
