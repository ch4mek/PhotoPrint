using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.Requests.MeetingRooms.Core.CoreDto;

namespace Modsen.Portal.Booking.Requests.MeetingRooms.Core;

internal interface IMeetingRoomReservationLoader
{
    Task<MeetingRoomReservationCoreDto[]> Load(int[] reservationIds, CancellationToken cancellationToken);
}

internal class MeetingRoomReservationLoader : IMeetingRoomReservationLoader
{
    private readonly IBookingDbContext _bookingDbContext;
    private readonly ITimeZoneConverter _officeTimeManager;

    public MeetingRoomReservationLoader(IBookingDbContext bookingDbContext, ITimeZoneConverter officeTimeManager)
    {
        _bookingDbContext = bookingDbContext;
        _officeTimeManager = officeTimeManager;
    }

    public async Task<MeetingRoomReservationCoreDto[]> Load(int[] reservationIds, CancellationToken cancellationToken)
    {
        var reservations = await _bookingDbContext.MeetingRoomReservations
            .Where(x => reservationIds.Contains(x.Id))
            .Where(x => !x.IsDeleted)
            .Select(x => new
            {
                x.Id,
                x.UserId,
                x.User!.FirstName,
                x.User.LastName,
                x.User.FirstNameRu,
                x.User.LastNameRu,
                x.MeetingRoomEntryId,
                x.MeetingRoomEntry!.MeetingRoomId,
                x.MeetingRoomEntry!.MeetingRoom!.Office!.TimeZone,
                MeetingRoomName = x.MeetingRoomEntry.MeetingRoom!.Name,
                x.MeetingRoomEntry.RepeatEveryWeek,
                x.ReservationFromUtc,
                x.ReservationToUtc,
                x.Comments
            })
            .ToArrayAsync(cancellationToken);

        return reservations
            .Select(reservation => new MeetingRoomReservationCoreDto
            {
                Id = reservation.Id,
                UserId = reservation.UserId,
                UserFirstName = reservation.FirstName,
                UserLastName = reservation.LastName,
                UserFirstNameRu = reservation.FirstNameRu,
                UserLastNameRu = reservation.LastNameRu,
                MeetingRoomEntryId = reservation.MeetingRoomEntryId,
                MeetingRoomEntryWeeksInInterval =  reservation.RepeatEveryWeek,
                MeetingRoomId = reservation.MeetingRoomId,
                MeetingRoomName = reservation.MeetingRoomName,
                ReservationFromUtc = reservation.ReservationFromUtc,
                ReservationToUtc = reservation.ReservationToUtc,
                ReservationFromOfficeDateTime = _officeTimeManager.ConvertUtcToOfficeTime(reservation.ReservationFromUtc, reservation.TimeZone!),
                ReservationToOfficeDateTime = _officeTimeManager.ConvertUtcToOfficeTime(reservation.ReservationToUtc, reservation.TimeZone!),
                Comments = reservation.Comments
            })
            .OrderBy(x => x.ReservationFromUtc)
            .ToArray();
    }
}
