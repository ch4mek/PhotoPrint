using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;

namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Output.Reports;

public sealed class ProjectWithTrackedTimeReportOutput
{
    public required string Name { get; init; }
    public string? Color { get; init; }
    public required int TotalTimeInMinutes { get; init; }

    internal static ProjectWithTrackedTimeReportOutput Of(ProjectWithTrackedTimeReportDto source)
    {
        return new ProjectWithTrackedTimeReportOutput
        {
            Name = source.Name,
            Color = source.Color,
            TotalTimeInMinutes = source.TotalTimeInMinutes
        };
    }
}
