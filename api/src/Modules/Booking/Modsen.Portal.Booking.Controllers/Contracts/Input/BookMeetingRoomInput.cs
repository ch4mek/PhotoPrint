namespace Modsen.Portal.Booking.Controllers.Contracts.Input;

public sealed class BookMeetingRoomInput
{
    public DateTime ReservationFromUtc { get; set; }
    public DateTime ReservationToUtc { get; set; }
    public DateOnly ReservationPeriodFromOfficeDate { get; set; }
    public DateOnly ReservationPeriodToOfficeDate { get; set; }
    public int UserId { get; set; }
    public int? RepeatReservationWeeks { get; set; }
}
