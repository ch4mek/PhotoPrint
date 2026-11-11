namespace Modsen.Portal.Booking.Requests.MeetingRooms.Core.CoreDto;

internal class MeetingRoomEntryCoreDto
{
    public int Id { get; set; }
    public int WeeksInInterval { get; set; }
    public bool IsRepetitiveBooking => WeeksInInterval > 0;
    public DateTime ReservationFromUtc { get; set; }
    public DateTime ReservationToUtc { get; set; }
    public DateTime ReservationFromOfficeTime { get; set; }
    public DateTime ReservationToOfficeTime { get; set; }
    public DateOnly ReservationPeriodFromOfficeDate { get; set; }
    public DateOnly ReservationPeriodToOfficeDate { get; set; }
    public DateOnly ReservationEndOfficeDate => ReservationPeriodToOfficeDate.AddDays(WeeksInInterval * 7);
    public int UserId { get; set; }
    public required string UserFirstName { get; set; }
    public required string UserLastName { get; set; }
    public string? UserFirstNameRu { get; set; }
    public string? UserLastNameRu { get; set; }
    public int MeetingRoomId { get; set; }
    public required string MeetingRoomName { get; set; }
    public MeetingRoomReservationCoreDto[] Reservations { get; set; } = [];
}
