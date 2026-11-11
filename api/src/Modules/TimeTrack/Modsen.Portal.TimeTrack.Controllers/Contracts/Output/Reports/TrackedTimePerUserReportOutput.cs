using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;

namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Output.Reports;

public sealed class TrackedTimePerUserReportOutput
{
    public required int UserId { get; init; }
    public required string LastName { get; init; }
    public required string FirstName { get; init; }
    public required bool IsActive { get; init; }
    public required int TotalTimeInMinutes { get; init; }
    public required int ProjectsCount { get; init; }
    public string? Color { get; init; }

    internal static TrackedTimePerUserReportOutput Of(TrackedTimePerUserReportDto source)
    {
        return new TrackedTimePerUserReportOutput
        {
            UserId = source.UserId,
            TotalTimeInMinutes = source.TotalTimeInMinutes,
            Color = source.Color,
            FirstName = source.FirstName,
            LastName = source.LastName,
            IsActive = source.IsActive,
            ProjectsCount = source.ProjectsCount
        };
    }
}
