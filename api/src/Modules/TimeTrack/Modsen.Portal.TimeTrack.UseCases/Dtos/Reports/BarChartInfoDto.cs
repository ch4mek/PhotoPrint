namespace Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;

internal class BarChartInfoDto
{
    public int ProjectId { get; init; }
    public required string ProjectName { get; init; }
    public string? ProjectColor { get; init; }
    public int? FactTrackedInMinutes { get; set; }
    public int? BillableTrackedInMinutes { get; set; }
    public DateOnly ReportForDate { get; set; }
    public int ReportYear { get; set; }
    public int ReportMonthOfYear { get; set; }
    public int ReportWeekOfYear { get; set; }
    public int ReportDayOfYear { get; set; }
}
