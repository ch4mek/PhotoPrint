namespace Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;

internal sealed class TrackedTimePerProjectReportDto
{
    public required int ProjectId { get; init; }
    public required string ProjectName { get; init; }
    public required string ClientName { get; init; }
    public required int TotalTimeInMinutes { get; init; }
    public required int TasksCount { get; init; }
    public string? Color { get; init; }
    public bool UserIsActive { get; init; }
}
