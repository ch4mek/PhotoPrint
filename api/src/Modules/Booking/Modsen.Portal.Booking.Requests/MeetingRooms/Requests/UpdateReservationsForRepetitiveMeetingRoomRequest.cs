using MediatR;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.DataAccess.Npgsql.Entities;
using Modsen.Portal.Booking.Requests.BookingPortalSettings;
using Modsen.Portal.Booking.Requests.MeetingRooms.Core;
using Modsen.Portal.Booking.Requests.MeetingRooms.Core.CoreDto;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.Booking.Requests.MeetingRooms.Requests;

internal class UpdateReservationsForRepetitiveMeetingRoomRequest :
    IRequestHandler<UpdateReservationsForRepetitiveMeetingRoomRequest.Command, int>
{
    private readonly IBookingDbContext _bookingDbContext;
    private readonly IClockService _clockService;
    private readonly IMeetingRoomReservationDateGenerator _reservationDateGenerator;
    private readonly IBookingPortalSettingsAccessor _bookingPortalSettingsAccessor;

    public UpdateReservationsForRepetitiveMeetingRoomRequest(
        IBookingDbContext bookingDbContext,
        IClockService clockService,
        IMeetingRoomReservationDateGenerator reservationDateGenerator,
        IBookingPortalSettingsAccessor bookingPortalSettingsAccessor)
    {
        _bookingDbContext = bookingDbContext;
        _clockService = clockService;
        _reservationDateGenerator = reservationDateGenerator;
        _bookingPortalSettingsAccessor = bookingPortalSettingsAccessor;
    }

    public async Task<int> Handle(Command request, CancellationToken cancellationToken)
    {
        var settings = await _bookingPortalSettingsAccessor.GetSettings(cancellationToken);
        var entries = await GetRepetitiveMeetingRoomEntries(cancellationToken);
        var maxReservationDate = _clockService.TodayUtc().AddMonths(settings.ReservationsMonthCount + 1);
        var reservations = new List<MeetingRoomReservationEntity>();

        foreach (var entry in entries)
        {
            var timeZone = entry.MeetingRoom?.Office?.TimeZone;

            if (timeZone.IsNullOrWhiteSpace())
            {
                continue;
            }

            var reservationDates = GenerateReservationDates(timeZone, settings.ReservationsMonthCount, maxReservationDate, entry);
            var missingDates = reservationDates
                .Where(date => !entry.MeetingRoomReservations
                    .Any(res => res.ReservationFromUtc == date.FromUtc &&
                        res.ReservationToUtc == date.ToUtc))
                .ToArray();
            foreach (var missingDate in missingDates)
            {
                reservations.Add(new MeetingRoomReservationEntity
                {
                    ReservationFromUtc = missingDate.FromUtc,
                    ReservationToUtc = missingDate.ToUtc,
                    CreatedById = entry.CreatedById,
                    CreatedAtUtc = entry.CreatedAtUtc,
                    UserId = entry.UserId,
                    MeetingRoomEntryId = entry.Id
                });
            }
        }

        _bookingDbContext.MeetingRoomReservations.AddRange(reservations);
        return await _bookingDbContext.SaveChangesAsync(cancellationToken);
    }

    private DateRangeCoreDto[] GenerateReservationDates(string timeZone, int reservationsMonthCount,
        DateOnly maxReservationDate, MeetingRoomEntryEntity entry)
    {
        return _reservationDateGenerator
            .GenerateReservationDatesUntil(maxReservationDate, new MeetingRoomReservationDateGeneratorRequest
            {
                OfficeTimeZone = timeZone,
                ReservationFromUtc = entry.ReservationFromUtc,
                ReservationToUtc = entry.ReservationToUtc,
                ReservationPeriodToOfficeDate = entry.ReservationPeriodToOfficeDate,
                ReservationPeriodFromOfficeDate = entry.ReservationPeriodFromOfficeDate,
                RepeatReservationWeeks = entry.RepeatEveryWeek,
                ReservationsMonthCount = reservationsMonthCount
            })
            .ToArray();
    }

    private Task<MeetingRoomEntryEntity[]> GetRepetitiveMeetingRoomEntries(CancellationToken token)
    {
        return _bookingDbContext.MeetingRoomEntries
            .Include(x => x.MeetingRoomReservations.Where(res => !res.IsDeleted))
            .Include(x => x.MeetingRoom)
            .ThenInclude(x => x!.Office)
            .Where(x => !x.IsDeleted)
            .Where(x => x.RepeatEveryWeek > 0)
            .Where(x => x.MeetingRoom!.IsActive)
            .ToArrayAsync(token);

    }

    internal sealed class Command : ICommand<int>, IBookingRequest, ITransactionalRequest;
}
