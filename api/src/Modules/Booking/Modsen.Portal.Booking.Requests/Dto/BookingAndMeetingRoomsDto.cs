namespace Modsen.Portal.Booking.Requests.Dto;

internal record BookingAndMeetingRoomsDto
{
    public List<BookingDto>? Bookings { get; init; } = [];
}
