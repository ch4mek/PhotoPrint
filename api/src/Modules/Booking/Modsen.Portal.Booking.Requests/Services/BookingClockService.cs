using Modsen.Portal.Booking.Requests.Services.Contracts;

namespace Modsen.Portal.Booking.Requests.Services;

internal class BookingClockService : IBookingClockService
{
    public DateTime RoundUpToHalfAnHourWithBufferSeconds(DateTime dateTime)
    {
        var date = DateOnly.FromDateTime(dateTime);
        var targetMinutes = dateTime.Minute - (dateTime.Minute % 30);

        return dateTime switch
        {
            { Minute: 0, Second: <= 59 } => new DateTime(date, new TimeOnly(dateTime.Hour - 1, 30, 0),
                DateTimeKind.Utc),

            { Minute: 30, Second: <= 59 } => new DateTime(date, new TimeOnly(dateTime.Hour, 0, 0),
                DateTimeKind.Utc),

            _ => new DateTime(date, new TimeOnly(dateTime.Hour, targetMinutes, 0),
                DateTimeKind.Utc)
        };
    }

    public DateTime RoundDownToMinute(DateTime dateTime)
    {
        var date = DateOnly.FromDateTime(dateTime);
        var time = new TimeOnly(dateTime.Hour, dateTime.Minute, 0);

        return new DateTime(date, time, dateTime.Kind);
    }
}
