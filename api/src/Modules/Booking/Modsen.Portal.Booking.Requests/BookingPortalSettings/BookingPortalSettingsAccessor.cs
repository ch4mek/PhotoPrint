using Modsen.Portal.Core.Services.PortalSettings;
using Modsen.Portal.Core.SettingNames;

namespace Modsen.Portal.Booking.Requests.BookingPortalSettings;

internal interface IBookingPortalSettingsAccessor
{
    Task<BookingPortalSettingsDto> GetSettings(CancellationToken token);
}

internal class BookingPortalSettingsAccessor :
    BasePortalSettingsAccessor<BookingPortalSettingsDto>,
    IBookingPortalSettingsAccessor
{
    public BookingPortalSettingsAccessor(IPortalSettingsAccessor portalSettingsAccessor)
        : base(portalSettingsAccessor) { }

    protected override string[] GetSettingNames()
    {
        return
        [
            BookingSettingNames.ReservationsMonthCount,
            PortalSettingNames.MinskTimezoneHours
        ];
    }

    protected override BookingPortalSettingsDto CreateSettingsModel(IReadOnlyCollection<PortalSettingValue> settings)
    {
        return new BookingPortalSettingsDto
        {
            ReservationsMonthCount = Get(BookingSettingNames.ReservationsMonthCount).GetInt(),
            MinskTimezoneHours = Get(PortalSettingNames.MinskTimezoneHours).GetInt()
        };
    }
}
