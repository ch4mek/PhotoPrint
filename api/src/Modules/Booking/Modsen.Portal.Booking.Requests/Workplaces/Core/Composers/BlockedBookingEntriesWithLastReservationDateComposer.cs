using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.Requests.Workplaces.Core.CoreDto;

namespace Modsen.Portal.Booking.Requests.Workplaces.Core.Composers;

internal interface IBlockedBookingEntriesWithLastReservationDateComposer
{
    Task<List<BlockedBookingEntryWithLastReservationDateDto>> Compose(DateTime startDate,
        CancellationToken cancellationToken);
}

internal class BlockedBookingEntriesWithLastReservationDateComposer :
    IBlockedBookingEntriesWithLastReservationDateComposer
{
    private readonly IBookingDbContext _dbContext;

    public BlockedBookingEntriesWithLastReservationDateComposer(IBookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<BlockedBookingEntryWithLastReservationDateDto>> Compose(DateTime startDate,
        CancellationToken cancellationToken)
    {
        return _dbContext.Reservations
            .Include(reservation => reservation.BookingEntry)
            .Where(reservation => !reservation.IsDeleted
                && !reservation.BookingEntry!.IsDeleted
                && reservation.BookingEntry!.IsBlock)
            .GroupBy(reservation => reservation.BookingEntry!.Id)
            .Where(group => group.Max(reservation => reservation.ReservationDate.Date) >= startDate)
            .Where(group => group.Any(reservation => reservation.ReservationDate.Date <= startDate))
            .Select(group => new BlockedBookingEntryWithLastReservationDateDto(
                group.Key,
                group.Max(y => y.ReservationDate)))
            .ToListAsync(cancellationToken);
    }
}
