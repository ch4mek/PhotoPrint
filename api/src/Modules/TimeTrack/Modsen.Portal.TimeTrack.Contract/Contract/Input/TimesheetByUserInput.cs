namespace Modsen.Portal.TimeTrack.Contract.Contract.Input;

public class TimesheetByUserInput
{
    public int UserId { get; init; }
    public int ProjectId { get; init; }
    public int Year { get; init; }
    public int Month { get; init; }
}
