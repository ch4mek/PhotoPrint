namespace Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;

internal sealed class TimePerPeriodReportDto
{
    public required DateTime Period { get; init; }
    public required int TotalTimeInMinutes { get; init; }
    public required IReadOnlyCollection<BarChartInfoResponseDto> Projects { get; init; } = [];
}
