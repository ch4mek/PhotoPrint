namespace Modsen.Portal.TimeTrack.UseCases.Queries.Core.Dto;

internal sealed class TimesheetRow
{
    public int UserId { get; set; }
    public int ProjectId { get; set; }
    public DateOnly ReportForDate { get; set; }
    public int MonthOfYear { get; set; }
    public int TotalFactTrackedInMinutesByProject { get; set; }
    public int TotalBillableTrackedInMinutesByProject { get; set; }
}
