namespace Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;

internal sealed class TrackedTimePerUserReportDto
{
    public required int UserId { get; init; }
    public required string LastName { get; init; }
    public required string FirstName { get; init; }
    public required bool IsActive { get; set; }
    public required int TotalTimeInMinutes { get; init; }
    public required int ProjectsCount { get; init; }
    public string? Color { get; init; }
}
