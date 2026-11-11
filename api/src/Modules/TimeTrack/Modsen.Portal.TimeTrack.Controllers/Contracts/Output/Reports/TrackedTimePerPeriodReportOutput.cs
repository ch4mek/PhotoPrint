using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;

namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Output.Reports;

public sealed class TrackedTimePerPeriodReportOutput
{
    public required int TotalTimeInMinutes { get; init; }
    public required IReadOnlyCollection<TimePerPeriodReportOutput> TimePerPeriod { get; init; } = [];

    internal static TrackedTimePerPeriodReportOutput Of(TrackedTimePerPeriodReportDto source)
    {
        return new TrackedTimePerPeriodReportOutput
        {
            TotalTimeInMinutes = source.TotalTimeInMinutes,
            TimePerPeriod = source.TimePerPeriod
                .Select(TimePerPeriodReportOutput.Of)
                .ToList(),
        };
    }
}
