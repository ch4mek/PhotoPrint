namespace Modsen.Portal.TimeTrack.UseCases.Queries.Core.Dto;

internal sealed class UserTimesheetDetailRow
{
    public int UserId { get; set; }
    public int ProjectId { get; set; }
    public int MonthOfYear { get; set; }
    public DateOnly UserOnProjectFrom { get; set; }
    public DateOnly? UserOnProjectTo { get; set; }
    public int TotalFactTrackedInMinutesByProject { get; set; }
    public int TotalBillableTrackedInMinutesByProject { get; set; }
}
