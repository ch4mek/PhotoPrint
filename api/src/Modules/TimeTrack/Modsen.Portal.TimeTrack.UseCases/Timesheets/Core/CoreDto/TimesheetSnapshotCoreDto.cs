namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;

internal sealed class TimesheetSnapshotCoreDto
{
    public int? Id { get; set; }

    public int? FactTrackedInMinutes { get; set; }

    public int? BillableTrackedInMinutes { get; set; }

    public DateOnly? ReportForDate { get; set; }
}
