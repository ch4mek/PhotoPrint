namespace Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;

internal sealed class UserWithTrackedTimeReportDto
{
    public required string LastName { get; init; }
    public required string FirstName { get; init; }
    public string? Color { get; init; }
    public required bool IsActive { get; set; }
    public required int TotalTimeInMinutes { get; init; }
}
