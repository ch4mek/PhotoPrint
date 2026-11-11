using MediatR;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.Requests.BookingPortalSettings;
using Modsen.Portal.Booking.Requests.Workplaces.Core.Composers;
using Modsen.Portal.Booking.Requests.Workplaces.Core.Generators;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Reservation = Modsen.Portal.Booking.DataAccess.Npgsql.DatabaseEntities.Reservation;

namespace Modsen.Portal.Booking.Requests.Workplaces.Requests;

internal class UpdateReservationsForBlockedWorkplacesRequest : IRequestHandler<UpdateReservationsForBlockedWorkplacesRequest.Command, int>
{
    private readonly IBlockedBookingEntriesWithLastReservationDateComposer _blockedBookingEntriesWithLastReservationDateComposer;
    private readonly IDatesBetweenExcludingStartGenerator _datesBetweenExcludingStartGenerator;
    private readonly IBookingPortalSettingsAccessor _bookingPortalSettingsAccessor;
    private readonly IBookingDbContext _bookingDbContext;
    private readonly IClockService _clockService;


    public UpdateReservationsForBlockedWorkplacesRequest(
        IClockService clockService,
        IBlockedBookingEntriesWithLastReservationDateComposer blockedBookingEntriesWithLastReservationDateComposer,
        IBookingPortalSettingsAccessor bookingPortalSettingsAccessor,
        IBookingDbContext bookingDbContext,
        IDatesBetweenExcludingStartGenerator datesBetweenExcludingStartGenerator)
    {
        _blockedBookingEntriesWithLastReservationDateComposer = blockedBookingEntriesWithLastReservationDateComposer;
        _datesBetweenExcludingStartGenerator = datesBetweenExcludingStartGenerator;
        _bookingPortalSettingsAccessor = bookingPortalSettingsAccessor;
        _bookingDbContext = bookingDbContext;
        _clockService = clockService;
    }

    public async Task<int> Handle(Command request, CancellationToken cancellationToken)
    {
        var settings = await _bookingPortalSettingsAccessor.GetSettings(cancellationToken);
        var nowDate = _clockService.UtcNow.Date;
        var endDate = nowDate.AddMonths(settings.ReservationsMonthCount);

        var entries = await _blockedBookingEntriesWithLastReservationDateComposer
            .Compose(nowDate, cancellationToken);

        foreach (var entry in entries)
        {
            if (entry.LastReservationDate > endDate)
            {
                continue;
            }

            var reservationDates = _datesBetweenExcludingStartGenerator.Generate(
                startDate: entry.LastReservationDate,
                finishDate: endDate).ToList();

            var reservations = reservationDates.Select(date => new Reservation
            {
                ReservationDate = date,
                BookingEntryId = entry.BookingEntryId
            }).ToList();

            _bookingDbContext.Reservations.AddRange(reservations);
        }

        await _bookingDbContext.SaveChangesAsync(cancellationToken);

        return entries.Count;
    }

    internal sealed class Command : ICommand<int>, IBookingRequest, ITransactionalRequest;
}
