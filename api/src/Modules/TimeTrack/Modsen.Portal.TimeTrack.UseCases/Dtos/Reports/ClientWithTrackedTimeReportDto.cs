namespace Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;

internal class ClientWithTrackedTimeReportDto
{
    public required string Name { get; init; }
    public string? Color { get; init; }
    public required int TotalTimeInMinutes { get; init; }
}
