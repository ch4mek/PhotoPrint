namespace Modsen.Portal.Booking.Requests.Booking.Core;

internal interface ICollisionsForDatesGenerator
{
    Task<List<DateTime>> GenerateCollisionsForDates(
    int weeksInInterval,
    int workplaceId,
    int userId,
    List<DateTime> dates,
    CancellationToken cancellationToken);
}

internal class CollisionsForDatesGenerator : ICollisionsForDatesGenerator
{
    private readonly IBookingsWithoutUsersOwnReservationsCollisionsResolver _bookingsWithoutUsersOwnReservationsCollisionsResolver;
    private readonly IBookingByDatesResolver _bookingsByDatesResolver;
    private readonly IBookingEntryLoader _bookingLoader;

    public CollisionsForDatesGenerator(
        IBookingsWithoutUsersOwnReservationsCollisionsResolver bookingsWithoutUsersOwnReservationsCollisionsResolver,
        IBookingByDatesResolver bookingsByDatesResolver,
        IBookingEntryLoader bookingLoader)
    {
        _bookingsWithoutUsersOwnReservationsCollisionsResolver = bookingsWithoutUsersOwnReservationsCollisionsResolver;
        _bookingsByDatesResolver = bookingsByDatesResolver;
        _bookingLoader = bookingLoader;
    }

    public async Task<List<DateTime>> GenerateCollisionsForDates(int weeksInInterval, int workplaceId, int userId, List<DateTime> dates, CancellationToken cancellationToken)
    {
        var bookingIds = await _bookingsByDatesResolver.ResolveBookingIdsByDatesAsync(dates, workplaceId, null, null, null, cancellationToken);
        var bookings = await _bookingLoader.Load(bookingIds, cancellationToken);

        var bookedDates = bookings.SelectMany(booking => booking.DateList).ToList();

        bookings = _bookingsWithoutUsersOwnReservationsCollisionsResolver.Resolve(
            bookings,
            bookedDates,
            dates,
            weeksInInterval,
            userId,
            guestName: null,
            workplaceId: null);

        var datesOfBookings = bookings.SelectMany(booking => booking.DateList).ToList();
        var collisionDates = datesOfBookings.Intersect(dates).OrderBy(d => d).ToList();

        return collisionDates;
    }
}
