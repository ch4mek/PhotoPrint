namespace Modsen.Portal.Booking.Requests.MeetingRooms.Core.CoreDto;

internal class MeetingRoomReservationCoreDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string UserFirstName { get; set; }
    public required string UserLastName { get; set; }
    public string? UserFirstNameRu { get; set; }
    public string? UserLastNameRu { get; set; }
    public int MeetingRoomId { get; set; }
    public required string MeetingRoomName { get; set; }
    public int MeetingRoomEntryId { get; set; }
    public int MeetingRoomEntryWeeksInInterval { get; set; }
    public bool IsMeetingRoomEntryRepetitiveBooking => MeetingRoomEntryWeeksInInterval > 0;
    public DateTime ReservationFromUtc { get; set; }
    public DateTime ReservationToUtc { get; set; }
    public DateTime ReservationFromOfficeDateTime { get; set; }
    public DateTime ReservationToOfficeDateTime { get; set; }

    public bool IsAllDay =>
        ReservationFromUtc.AddDays(1) == ReservationToUtc ||
        ReservationFromUtc.AddDays(1) == ReservationToUtc.AddSeconds(1);

    public string? Comments { get; set; }
}
