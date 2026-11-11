using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.Requests.Booking.Core.CoreDto;

namespace Modsen.Portal.Booking.Requests.Booking.Core;

internal interface IBookingEntryLoader
{
    Task<BookingEntryCoreDto?> Load(int bookingId, CancellationToken cancellationToken);
    Task<List<BookingEntryCoreDto>> Load(int[] bookingIds, CancellationToken cancellationToken);
}

internal class BookingEntryLoader : IBookingEntryLoader
{
    private readonly IBookingDbContext _bookingDbContext;

    public BookingEntryLoader(IBookingDbContext bookingDbContext)
    {
        _bookingDbContext = bookingDbContext;
    }

    public async Task<BookingEntryCoreDto?> Load(int bookingId, CancellationToken cancellationToken)
    {
        var bookings = await Load([bookingId], cancellationToken);
        return bookings.FirstOrDefault();
    }

    public async Task<List<BookingEntryCoreDto>> Load(int[] bookingIds, CancellationToken cancellationToken)
    {
        var bookings = await _bookingDbContext.Bookings
            .AsNoTracking()
            .AsSplitQuery()
            .Where(x => bookingIds.Contains(x.Id))
            .Where(b => !b.IsDeleted)
            .Include(b => b.User)
            .Include(b => b.Workplace)
            .Include(b => b.Reservations)
            .Include(b => b.Notifications)
            .ToListAsync(cancellationToken);

        return bookings.Select(x => new BookingEntryCoreDto
        {
            BookingId = x.Id,
            UserId = x.UserId,
            FirstName = x.User?.FirstName ?? string.Empty,
            LastName = x.User?.LastName ?? string.Empty,
            FirstNameRu = x.User?.FirstNameRu,
            LastNameRu = x.User?.LastNameRu,
            IsForGuest = x.IsForGuest,
            GuestName = x.GuestName,
            DateList = x.Reservations
                .Select(r => r.ReservationDate)
                .ToList(),
            IsBlock = x.IsBlock,
            WorkplaceId = x.WorkplaceId,
            WorkplaceOfficeId = x.Workplace?.OfficeId ?? 0,
            WorkplaceNumber = x.Workplace?.WorkplaceNumber ?? 0,
            WorkspaceId = x.Workplace?.WorkspaceId,
            WeeksInInterval = x.WeeksInInterval,
        }).ToList();
    }
}
