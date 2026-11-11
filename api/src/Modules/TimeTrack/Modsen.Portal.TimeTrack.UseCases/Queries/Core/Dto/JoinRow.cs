namespace Modsen.Portal.TimeTrack.UseCases.Queries.Core.Dto;

internal sealed class JoinRow
{
    public DateOnly UserOnProjectFrom { get; set; }
    public DateOnly? UserOnProjectTo { get; set; }
    public int UserId { get; set; }
    public int ProjectId { get; set; }
    public int MonthOfYear { get; set; }
    public DateOnly ReportForDate { get; set; }
    public int TotalBillableTrackedInMinutesByProject { get; set; }
    public int TotalFactTrackedInMinutesByProject { get; set; }
}
