using Modsen.Portal.Booking.Requests.Booking.Core.CoreDto;

namespace Modsen.Portal.Booking.Requests.Booking.Core;

internal interface IBookingsWithoutUsersOwnReservationsCollisionsResolver
{
    List<BookingEntryCoreDto> Resolve(
        List<BookingEntryCoreDto> bookingEntities,
        List<DateTime> bookedDates,
        List<DateTime> requestedDates,
        int weeksInInterval,
        int? userId,
        string? guestName,
        int? workplaceId);
}

internal class BookingsWithoutUsersOwnReservationsCollisionsResolver : IBookingsWithoutUsersOwnReservationsCollisionsResolver
{
    public List<BookingEntryCoreDto> Resolve(
        List<BookingEntryCoreDto> bookingEntities,
        List<DateTime> bookedDates,
        List<DateTime> requestedDates,
        int weeksInInterval,
        int? userId,
        string? guestName,
        int? workplaceId)
    {
        var areBookingsIncludeAnyMultipleOrRepetitiveBooking = !bookingEntities.Any(booking =>
        booking.DateList.Count > 1
        || booking.WeeksInInterval > 0);

        var areBookingsSingleOnes = bookedDates.Count == 1 &&
                                        requestedDates.Count == 1;

        var canCollisionsBeSkipped = areBookingsIncludeAnyMultipleOrRepetitiveBooking
                                      && weeksInInterval == 0
                                      && !bookedDates.Except(requestedDates).Any()
                                      && !areBookingsSingleOnes;

        if (canCollisionsBeSkipped && workplaceId is not null)
        {
            bookingEntities = bookingEntities.Where(booking => booking.WorkplaceId != workplaceId).ToList();
        }

        if (canCollisionsBeSkipped && userId is not null)
        {
            bookingEntities = bookingEntities.Where(booking => booking.UserId != userId).ToList();
        }

        if (canCollisionsBeSkipped && guestName is not null)
        {
            bookingEntities = bookingEntities.Where(booking => booking.GuestName != guestName).ToList();
        }

        return bookingEntities;
    }
}
