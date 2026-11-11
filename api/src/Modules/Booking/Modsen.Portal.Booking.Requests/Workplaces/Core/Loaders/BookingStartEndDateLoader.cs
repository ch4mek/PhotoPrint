using Modsen.Portal.Booking.Requests.BookingPortalSettings;
using Modsen.Portal.Core.Services;

namespace Modsen.Portal.Booking.Requests.Workplaces.Core.Loaders;

internal interface IBookingStartEndDateLoader
{
    Task<(DateTime Start, DateTime End)> LoadAsync(CancellationToken cancellationToken);
}

internal sealed class BookingStartEndDateLoader : IBookingStartEndDateLoader
{
    private readonly IBookingPortalSettingsAccessor _bookingPortalSettingsAccessor;
    private readonly IClockService _clockService;

    public BookingStartEndDateLoader(
        IBookingPortalSettingsAccessor bookingPortalSettingsAccessor,
        IClockService clockService)
    {
        _bookingPortalSettingsAccessor = bookingPortalSettingsAccessor;
        _clockService = clockService;
    }

    public async Task<(DateTime Start, DateTime End)> LoadAsync(CancellationToken cancellationToken)
    {
        var settings = await _bookingPortalSettingsAccessor.GetSettings(cancellationToken);

        var start = _clockService.GetMinskNow(settings.MinskTimezoneHours);
        var end = start.AddMonths(settings.ReservationsMonthCount);

        return (start, end);
    }
}
