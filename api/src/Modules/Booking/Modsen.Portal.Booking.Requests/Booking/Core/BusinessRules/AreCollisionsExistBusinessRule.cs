using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;

namespace Modsen.Portal.Booking.Requests.Booking.Core.BusinessRules;

internal interface IAreCollisionsExistBusinessRule
{
    Task<bool> AreCollisionsForWorkplaceByDatesExist(
        int weeksInInterval,
        int? userId,
        string? guestName,
        int workplaceId,
        List<DateTime> dates,
        CancellationToken cancellationToken);

    Task<bool> AreCollisionsForUserByDatesExist(
        int weeksInInterval,
        int userId,
        int workplaceId,
        int officeId,
        List<DateTime> dates,
        CancellationToken cancellationToken);

    Task<bool> AreCollisionsForGuestByDatesExist(
        int weeksInInterval,
        string? guestName,
        int workplaceId,
        int officeId,
        List<DateTime> dates,
        CancellationToken cancellationToken);
}

internal class AreCollisionsExistBusinessRule : IAreCollisionsExistBusinessRule
{
    private readonly IBookingsWithoutUsersOwnReservationsCollisionsResolver _bookingsWithoutUsersOwnReservationsCollisionsResolver;
    private readonly IBookingByDatesResolver _bookingsByDatesResolver;
    private readonly IBookingEntryLoader _bookingLoader;

    public AreCollisionsExistBusinessRule(
        IBookingsWithoutUsersOwnReservationsCollisionsResolver bookingsWithoutUsersOwnReservationsCollisionsResolver,
        IBookingByDatesResolver bookingsByDatesResolver,
        IBookingEntryLoader bookingLoader)
    {
        _bookingsWithoutUsersOwnReservationsCollisionsResolver = bookingsWithoutUsersOwnReservationsCollisionsResolver;
        _bookingsByDatesResolver = bookingsByDatesResolver;
        _bookingLoader = bookingLoader;
    }

    public async Task<bool> AreCollisionsForWorkplaceByDatesExist(int weeksInInterval, int? userId, string? guestName, int workplaceId, List<DateTime> dates, CancellationToken cancellationToken)
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
            guestName,
            workplaceId: null);

        if (bookings.Count != 0)
        {
            throw new EntityConflictException(PortalMessages.BookingExistsForWorkplace);
        }

        return false;
    }

    public async Task<bool> AreCollisionsForUserByDatesExist(int weeksInInterval, int userId, int workplaceId, int officeId, List<DateTime> dates, CancellationToken cancellationToken)
    {
        var bookingIds = await _bookingsByDatesResolver.ResolveBookingIdsByDatesAsync(dates, null, userId, officeId, null, cancellationToken);
        var bookings = await _bookingLoader.Load(bookingIds, cancellationToken);

        bookings = bookings.Where(booking => !booking.IsForGuest).ToList();

        var bookedDates = bookings.SelectMany(booking => booking.DateList).ToList();

        bookings = _bookingsWithoutUsersOwnReservationsCollisionsResolver.Resolve(
            bookings,
            bookedDates,
            dates,
            weeksInInterval,
            userId: null,
            guestName: null,
            workplaceId);

        if (bookings.Count != 0)
        {
            throw new EntityConflictException(PortalMessages.AttemptToCreateSeveralBookings);
        }

        return false;
    }

    public async Task<bool> AreCollisionsForGuestByDatesExist(int weeksInInterval, string? guestName, int workplaceId, int officeId, List<DateTime> dates, CancellationToken cancellationToken)
    {
        var bookingIds = await _bookingsByDatesResolver.ResolveBookingIdsByDatesAsync(dates, null, null, officeId, guestName, cancellationToken);
        var bookings = await _bookingLoader.Load(bookingIds, cancellationToken);

        var bookedDates = bookings.SelectMany(booking => booking.DateList).ToList();

        bookings = _bookingsWithoutUsersOwnReservationsCollisionsResolver.Resolve(
            bookings,
            bookedDates,
            dates,
            weeksInInterval,
            userId: null,
            guestName: null,
            workplaceId);

        if (bookings.Count != 0)
        {
            throw new EntityConflictException(PortalMessages.AttemptToCreateSeveralBookingsForGuest);
        }

        return false;
    }
}
