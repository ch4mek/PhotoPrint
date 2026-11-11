namespace Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;

internal sealed class TrackedTaskReportDto
{
    public required DateOnly ReportForDate { get; init; }
    public required string TaskName { get; init; }
    public required int TrackedTimeInMinutes { get; init; }
    public string? Color { get; init; }
}
