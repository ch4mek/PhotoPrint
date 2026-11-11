using Modsen.Portal.Core.Booking;

namespace Modsen.Portal.Booking.Requests.MeetingRooms.Requests.RequestDto
{
    internal class MeetingRoomItemRequestDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public MeetingRoomItemType Type { get; set; }
        public int Count { get; set; }
    }
}
