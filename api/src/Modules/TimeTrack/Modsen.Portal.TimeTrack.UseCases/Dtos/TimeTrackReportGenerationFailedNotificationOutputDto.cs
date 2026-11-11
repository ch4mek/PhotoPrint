namespace Modsen.Portal.TimeTrack.UseCases.Dtos;

internal class TimeTrackReportGenerationFailedNotificationOutputDto
{
    public required string ReportName { get; set; }
    public required string DocumentType { get; set; }
}
