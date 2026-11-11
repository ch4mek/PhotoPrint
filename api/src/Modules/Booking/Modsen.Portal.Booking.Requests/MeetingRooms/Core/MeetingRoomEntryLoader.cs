using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.Requests.MeetingRooms.Core.CoreDto;

namespace Modsen.Portal.Booking.Requests.MeetingRooms.Core;

internal interface IMeetingRoomEntryLoader
{
    Task<MeetingRoomEntryCoreDto?> Load(int entryId, CancellationToken cancellationToken);
    Task<MeetingRoomEntryCoreDto[]> Load(int[] entryIds, CancellationToken cancellationToken);
}

internal class MeetingRoomEntryLoader : IMeetingRoomEntryLoader
{
    private readonly IBookingDbContext _bookingDbContext;
    private readonly ITimeZoneConverter _officeTimeManager;
    private readonly IMeetingRoomReservationLoader _meetingRoomReservationLoader;

    public MeetingRoomEntryLoader(IBookingDbContext bookingDbContext, ITimeZoneConverter officeTimeManager,
        IMeetingRoomReservationLoader meetingRoomReservationLoader)
    {
        _bookingDbContext = bookingDbContext;
        _officeTimeManager = officeTimeManager;
        _meetingRoomReservationLoader = meetingRoomReservationLoader;
    }

    public async Task<MeetingRoomEntryCoreDto?> Load(int entryId, CancellationToken cancellationToken)
    {
        var entries = await Load([entryId], cancellationToken);
        return entries.FirstOrDefault();
    }

    public async Task<MeetingRoomEntryCoreDto[]> Load(int[] entryIds, CancellationToken cancellationToken)
    {
        var entries = await _bookingDbContext.MeetingRoomEntries
            .Where(x => entryIds.Contains(x.Id))
            .Where(x => !x.IsDeleted)
            .Select(entry => new
            {
                entry.Id,
                entry.UserId,
                entry.User!.FirstName,
                entry.User.LastName,
                entry.User.FirstNameRu,
                entry.User.LastNameRu,
                entry.MeetingRoomId,
                MeetingRoomName = entry.MeetingRoom!.Name,
                entry.ReservationFromUtc,
                entry.ReservationToUtc,
                entry.RepeatEveryWeek,
                entry.ReservationPeriodFromOfficeDate,
                entry.ReservationPeriodToOfficeDate,
                entry.MeetingRoom!.Office!.TimeZone,
                ReservationIds = entry.MeetingRoomReservations.Select(x => x.Id).ToArray()
            })
            .ToArrayAsync(cancellationToken);
        var reservationIds = entries
            .SelectMany(x => x.ReservationIds)
            .Distinct()
            .ToArray();
        var reservations = await _meetingRoomReservationLoader.Load(reservationIds, cancellationToken);

        return entries
            .Select(entry => new MeetingRoomEntryCoreDto
            {
                Id = entry.Id,
                UserId = entry.UserId,
                UserFirstName = entry.FirstName,
                UserLastName = entry.LastName,
                UserFirstNameRu = entry.FirstNameRu,
                UserLastNameRu = entry.LastNameRu,
                MeetingRoomId = entry.MeetingRoomId,
                MeetingRoomName = entry.MeetingRoomName,
                ReservationFromUtc = entry.ReservationFromUtc,
                ReservationToUtc = entry.ReservationToUtc,
                ReservationFromOfficeTime = _officeTimeManager.ConvertUtcToOfficeTime(entry.ReservationFromUtc, entry.TimeZone!),
                ReservationToOfficeTime = _officeTimeManager.ConvertUtcToOfficeTime(entry.ReservationToUtc, entry.TimeZone!),
                WeeksInInterval = entry.RepeatEveryWeek,
                ReservationPeriodFromOfficeDate = entry.ReservationPeriodFromOfficeDate,
                ReservationPeriodToOfficeDate = entry.ReservationPeriodToOfficeDate,
                Reservations = reservations.Where(x => x.MeetingRoomEntryId == entry.Id).ToArray()
            })
            .OrderBy(x => x.ReservationFromUtc)
            .ToArray();
    }
}
