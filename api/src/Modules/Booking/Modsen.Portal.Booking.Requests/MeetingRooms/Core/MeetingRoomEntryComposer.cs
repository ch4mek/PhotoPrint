using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.DataAccess.Npgsql.Entities;
using Modsen.Portal.Booking.Requests.MeetingRooms.Core.CoreDto;

namespace Modsen.Portal.Booking.Requests.MeetingRooms.Core;

internal interface IMeetingRoomEntryComposer
{
    Task<MeetingRoomEntryEntity> Compose(MeetingRoomEntryComposerRequest request, CancellationToken token);
}

internal class MeetingRoomEntryComposer : IMeetingRoomEntryComposer
{
    private readonly IBookingDbContext _bookingDbContext;

    public MeetingRoomEntryComposer(IBookingDbContext bookingDbContext)
    {
        _bookingDbContext = bookingDbContext;
    }

    public async Task<MeetingRoomEntryEntity> Compose(MeetingRoomEntryComposerRequest request,
        CancellationToken token)
    {
        var reservations = request.ReservationDates
            .Select(date => new MeetingRoomReservationEntity
            {
                UserId = request.UserId,
                ReservationFromUtc = date.FromUtc,
                ReservationToUtc = date.ToUtc,
                Comments = null
            })
            .ToArray();
        var entry = new MeetingRoomEntryEntity
        {
            MeetingRoomId = request.MeetingRoomId,
            RepeatEveryWeek = request.RepeatReservationWeeks ?? 0,
            ReservationFromUtc = request.ReservationFromUtc,
            ReservationToUtc = request.ReservationToUtc,
            ReservationPeriodFromOfficeDate = request.ReservationPeriodFromOfficeDate,
            ReservationPeriodToOfficeDate = request.ReservationPeriodToOfficeDate,
            UserId = request.UserId,
            MeetingRoomReservations = reservations
        };

        _bookingDbContext.MeetingRoomEntries.Add(entry);
        await _bookingDbContext.SaveChangesAsync(token);

        return entry;
    }
}

internal class MeetingRoomEntryComposerRequest
{
    public DateTime ReservationFromUtc { get; set; }
    public DateTime ReservationToUtc { get; set; }
    public DateOnly ReservationPeriodFromOfficeDate { get; set; }
    public DateOnly ReservationPeriodToOfficeDate { get; set; }
    public int UserId { get; set; }
    public int MeetingRoomId { get; set; }
    public int? RepeatReservationWeeks { get; set; }
    public IReadOnlyCollection<DateRangeCoreDto> ReservationDates { get; set; } = [];
}
