namespace Modsen.Portal.Booking.Requests.MeetingRooms.Core.CoreDto;

internal record OfficeDayRangeCoreDto
{
    public DateOnly OfficeTimeZoneDay { get; init; }
    public DateTime OfficeTimeDayFrom { get; init; }
    public DateTime OfficeTimeDayTo { get; init; }
    public DateTime UtcDayFrom { get; init; }
    public DateTime UtcDayTo { get; init; }
}
