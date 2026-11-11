using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;

namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Output.Reports;

public sealed class TrackedTaskReportOutput
{
    public required DateOnly ReportForDate { get; init; }
    public required string TaskName { get; init; }
    public required int TrackedTimeInMinutes { get; init; }
    public string? Color { get; init; }

    internal static TrackedTaskReportOutput Of(TrackedTaskReportDto source)
    {
        return new TrackedTaskReportOutput
        {
            ReportForDate = source.ReportForDate,
            TaskName = source.TaskName,
            TrackedTimeInMinutes = source.TrackedTimeInMinutes,
            Color = source.Color
        };
    }
}
