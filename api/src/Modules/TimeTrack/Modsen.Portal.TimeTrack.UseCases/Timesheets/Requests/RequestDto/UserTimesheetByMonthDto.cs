namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

internal class UserTimesheetByMonthDto
{
    public int UserId { get; init; }
    public int ProjectId { get; init; }
    public int BillableTrackedSum { get; init; }
    public int Year { get; init; }
    public int Month { get; init; }
}
