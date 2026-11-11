using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;

namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Output.Reports;

public sealed class TrackedTimePerProjectReportOutput
{
    public required int ProjectId { get; init; }
    public required string ProjectName { get; init; }
    public required string ClientName { get; init; }
    public required int TotalTimeInMinutes { get; init; }
    public required int TasksCount { get; init; }
    public string? Color { get; init; }
    public bool UserIsActive { get; init; }

    internal static TrackedTimePerProjectReportOutput Of(TrackedTimePerProjectReportDto source)
    {
        return new TrackedTimePerProjectReportOutput
        {
            ProjectId = source.ProjectId,
            ProjectName = source.ProjectName,
            ClientName = source.ClientName,
            Color = source.Color,
            TasksCount = source.TasksCount,
            TotalTimeInMinutes = source.TotalTimeInMinutes,
            UserIsActive = source.UserIsActive
        };
    }
}
