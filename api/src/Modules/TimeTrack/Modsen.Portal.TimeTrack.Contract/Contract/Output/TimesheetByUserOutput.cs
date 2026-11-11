namespace Modsen.Portal.TimeTrack.Contract.Contract.Output;

public class TimesheetByUserOutput
{
    public int UserId { get; init; }
    public int ProjectId { get; init; }
    public int BillableTrackedSum { get; init; }
}
