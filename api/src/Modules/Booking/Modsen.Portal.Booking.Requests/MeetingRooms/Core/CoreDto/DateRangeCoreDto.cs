namespace Modsen.Portal.Booking.Requests.MeetingRooms.Core.CoreDto;

internal class DateRangeCoreDto
{
    public required DateTime FromUtc { get; init; }
    public required DateTime ToUtc { get; init; }
}
