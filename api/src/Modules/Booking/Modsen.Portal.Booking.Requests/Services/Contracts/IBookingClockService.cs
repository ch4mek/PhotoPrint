namespace Modsen.Portal.Booking.Requests.Services.Contracts;

internal interface IBookingClockService
{
    public DateTime RoundUpToHalfAnHourWithBufferSeconds(DateTime dateTime);
    public DateTime RoundDownToMinute(DateTime dateTime);
}
