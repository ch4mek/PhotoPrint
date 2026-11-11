using Modsen.Portal.Booking.Requests.MeetingRooms.Core.CoreDto;

namespace Modsen.Portal.Booking.Requests.MeetingRooms.Requests.RequestDto;

internal class MeetingRoomReservationGroupRequestDto
{
    public int MeetingRoomId { get; set; }
    public required string MeetingRoomName { get; set; }
    public int CountOfSeats { get; set; }
    public MeetingRoomItemRequestDto[] Equipments { get; set; } = [];
    public MeetingRoomReservationCoreDto[] Reservations { get; set; } = [];
}
