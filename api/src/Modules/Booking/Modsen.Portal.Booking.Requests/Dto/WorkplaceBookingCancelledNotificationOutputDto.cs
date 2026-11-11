using Modsen.Portal.Core.Communication.Notifications;

namespace Modsen.Portal.Booking.Requests.Dto;

public class WorkplaceBookingCancelledNotificationOutputDto : INotificationOutput
{
    public IReadOnlyCollection<DateOnly> DateList { get; set; } = [];
    public int WorkplaceNumber { get; set; }
    public int WorkplaceId { get; set; }
    public int? WorkspaceId { get; set; }
    public int WorkplaceOfficeId { get; set; }
    public int BookingEntryId { get; set; }
    public bool IsBlock { get; set; }
    public string? Message { get; set; }
}
