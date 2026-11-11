namespace Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;

internal sealed class TrackedTimePerPeriodReportDto
{
    public required int TotalTimeInMinutes { get; set; }
    public required IReadOnlyCollection<TimePerPeriodReportDto> TimePerPeriod { get; set; }
    public required IReadOnlyCollection<int> Projects { get; set; }
}
