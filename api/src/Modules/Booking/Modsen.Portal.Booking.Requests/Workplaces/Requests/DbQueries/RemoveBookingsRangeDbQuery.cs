using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;

namespace Modsen.Portal.Booking.Requests.Workplaces.Requests.DbQueries;

internal interface IRemoveBookingsRangeDbQuery
{
    Task RemoveAsync(List<int> ids, CancellationToken cancellationToken);
}

internal sealed class RemoveBookingsRangeDbQuery : IRemoveBookingsRangeDbQuery
{
    private readonly IBookingDbContext _bookingDbContext;

    public RemoveBookingsRangeDbQuery(IBookingDbContext bookingDbContext)
    {
        _bookingDbContext = bookingDbContext;
    }

    public async Task RemoveAsync(List<int> ids, CancellationToken cancellationToken)
    {
        await _bookingDbContext.Bookings
            .Where(bookingEntry => ids.Contains(bookingEntry.Id))
            .ForEachAsync(bookingEntry => bookingEntry.IsDeleted = true, cancellationToken);

        await _bookingDbContext.Reservations
            .Where(reservation => ids.Contains(reservation.BookingEntryId))
            .ForEachAsync(reservation => reservation.IsDeleted = true, cancellationToken);

        await _bookingDbContext.SaveChangesAsync(cancellationToken);
    }
}
