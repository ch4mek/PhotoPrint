namespace Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;

internal record ProjectWithTrackedTimeReportDto(
    string Name,
    string? Color,
    int TotalTimeInMinutes);
