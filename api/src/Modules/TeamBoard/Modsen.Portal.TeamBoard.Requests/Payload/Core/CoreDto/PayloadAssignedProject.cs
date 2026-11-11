namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

public class PayloadAssignedProject
{
    public int ProjectId { get; set; }
    public bool IsActiveAssignement { get; set; }
    public bool IsTimeOnProjectBillable { get; set; }
    public bool IsFutureActiveAssignment { get; set; }
}
