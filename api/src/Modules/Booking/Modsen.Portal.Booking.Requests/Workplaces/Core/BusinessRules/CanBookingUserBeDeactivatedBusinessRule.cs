using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Core.Services;

namespace Modsen.Portal.Booking.Requests.Workplaces.Core.BusinessRules;

internal interface ICanBookingUserBeDeactivatedBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(int userId, CancellationToken cancellationToken);
}

internal class CanBookingUserBeDeactivatedBusinessRule : ICanBookingUserBeDeactivatedBusinessRule
{
    private readonly IBookingDbContext _bookingDbContext;
    private readonly IClockService _clockService;

    public CanBookingUserBeDeactivatedBusinessRule(
        IBookingDbContext bookingDbContext,
        IClockService clockService)
    {
        _bookingDbContext = bookingDbContext;
        _clockService = clockService;
    }

    public async Task<bool> IsCorrespondsToBusiness(int userId, CancellationToken cancellationToken)
    {
        var todayMidnight = _clockService.TodayStartOfDayUtc();

        return !await HasActiveWorkplaceReservations() &&
               !await HasActiveMeetingRoomReservations();

        Task<bool> HasActiveWorkplaceReservations()
        {
            return _bookingDbContext.Reservations
                .Where(x => !x.IsDeleted)
                .Where(x => x.BookingEntry!.UserId == userId)
                .Where(x => x.ReservationDate >= todayMidnight)
                .AnyAsync(cancellationToken);
        }

        Task<bool> HasActiveMeetingRoomReservations()
        {
            return _bookingDbContext.MeetingRoomReservations
                .Where(x => !x.IsDeleted)
                .Where(x => x.UserId == userId)
                .Where(x => x.ReservationFromUtc >= todayMidnight)
                .AnyAsync(cancellationToken);
        }
    }
}
