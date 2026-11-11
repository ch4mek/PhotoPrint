using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;

namespace Modsen.Portal.Booking.Requests.Booking.Core;

internal interface IBookingByDatesResolver
{
    Task<int[]> ResolveBookingIdsByDatesAsync(
        IReadOnlyCollection<DateTime> dates,
        int? workplaceId,
        int? userId,
        int? officeId,
        string? guestName,
        CancellationToken cancellationToken);
}

internal class BookingByDatesResolver : IBookingByDatesResolver
{
    private readonly IBookingDbContext _bookingDbContext;

    public BookingByDatesResolver(IBookingDbContext bookingDbContext)
    {
        _bookingDbContext = bookingDbContext;
    }

    public async Task<int[]> ResolveBookingIdsByDatesAsync(
        IReadOnlyCollection<DateTime> dates,
        int? workplaceId,
        int? userId,
        int? officeId,
        string? guestName,
        CancellationToken cancellationToken)
    {
        var bookingIds = await _bookingDbContext.Bookings
            .Where(b => workplaceId == null || b.Workplace!.Id == workplaceId)
            .Where(b => userId == null || b.UserId == userId)
            .Where(b => officeId == null || b.Workplace!.OfficeId == officeId)
            .Where(b => guestName == null || b.GuestName == guestName)
            .Where(b => b.Reservations
                .Any(r => dates.Any(d => d == r.ReservationDate)))
            .Select(x => x.Id)
            .ToArrayAsync(cancellationToken);

        return bookingIds;
    }
}
