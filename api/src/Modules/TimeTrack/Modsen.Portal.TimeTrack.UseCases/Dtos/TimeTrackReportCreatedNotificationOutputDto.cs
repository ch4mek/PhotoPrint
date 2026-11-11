using Modsen.Portal.Core.Communication.Notifications;

namespace Modsen.Portal.TimeTrack.UseCases.Dtos;

internal class TimeTrackReportCreatedNotificationOutputDto : INotificationOutput
{
    public required string ReportName { get; set; }
    public string? ReportFileId { get; set; }
    public required string DocumentType { get; set; }
    public required double SizeInKilobytes { get; set; }
}
