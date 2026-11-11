namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;

internal sealed class TimesheetWithTrackedTimeCoreDto
{
    public int Id { get; set; }
    public DateOnly ReportForDate { get; set; }
    public int TrackedInMinutes { get; set; }
}
