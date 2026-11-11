using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;

namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Output.Reports;

public sealed class TimePerPeriodReportOutput
{
    public required DateTime Period { get; init; }
    public required int TotalTimeInMinutes { get; init; }
    public required IReadOnlyCollection<BarChartInfoResponseDto> Projects { get; init; } = [];

    internal static TimePerPeriodReportOutput Of(TimePerPeriodReportDto source)
    {
        return new TimePerPeriodReportOutput()
        {
            Period = source.Period,
            TotalTimeInMinutes = source.TotalTimeInMinutes,
            Projects = source.Projects,
        };
    }
}
