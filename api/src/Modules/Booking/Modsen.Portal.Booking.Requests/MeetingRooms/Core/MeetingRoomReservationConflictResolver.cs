using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.Requests.MeetingRooms.Core.CoreDto;

namespace Modsen.Portal.Booking.Requests.MeetingRooms.Core;

internal interface IMeetingRoomReservationConflictResolver
{
    Task<int[]> ResolveConflictsFor(
        IReadOnlyCollection<DateRangeCoreDto> reservationDates,
        int meetingRoomId,
        int userId,
        CancellationToken token);
}

internal class MeetingRoomReservationReservationConflictResolver : IMeetingRoomReservationConflictResolver
{
    private readonly IBookingDbContext _bookingDbContext;

    public MeetingRoomReservationReservationConflictResolver(IBookingDbContext bookingDbContext)
    {
        _bookingDbContext = bookingDbContext;
    }

    public async Task<int[]> ResolveConflictsFor(
        IReadOnlyCollection<DateRangeCoreDto> reservationDates,
        int meetingRoomId,
        int userId,
        CancellationToken token)
    {
        var officeId = await _bookingDbContext.MeetingRooms
            .Where(mr => mr.Id == meetingRoomId)
            .Select(mr => mr.OfficeId)
            .FirstOrDefaultAsync(token);

        var reservations = await _bookingDbContext.MeetingRoomReservations
            .Where(x => x.MeetingRoomEntry!.MeetingRoom!.OfficeId == officeId)
            .Where(x => x.MeetingRoomEntry!.MeetingRoomId == meetingRoomId || x.MeetingRoomEntry.UserId == userId)
            .Where(x => !x.IsDeleted)
            .Select(x => new { x.Id, x.ReservationFromUtc, x.ReservationToUtc })
            .ToArrayAsync(token);

        return reservations
            .Where(res => reservationDates
                .Any(date =>
                    (date.FromUtc > res.ReservationFromUtc && date.ToUtc < res.ReservationToUtc) ||
                    (date.FromUtc < res.ReservationFromUtc && date.ToUtc > res.ReservationToUtc) ||
                    (date.FromUtc > res.ReservationFromUtc && date.FromUtc < res.ReservationToUtc) ||
                    (date.FromUtc < res.ReservationFromUtc && date.ToUtc == res.ReservationToUtc) ||
                    (date.ToUtc > res.ReservationFromUtc && date.ToUtc < res.ReservationToUtc) ||
                    (date.FromUtc == res.ReservationFromUtc && date.ToUtc == res.ReservationToUtc) ||
                    (date.FromUtc == res.ReservationFromUtc && date.ToUtc > res.ReservationToUtc))
            )
            .Select(x => x.Id)
            .Distinct()
            .ToArray();
    }
}
