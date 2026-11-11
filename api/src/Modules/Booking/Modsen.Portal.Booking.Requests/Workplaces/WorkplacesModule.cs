using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Booking.Requests.Workplaces.Core.BusinessRules;
using Modsen.Portal.Booking.Requests.Workplaces.Core.Composers;
using Modsen.Portal.Booking.Requests.Workplaces.Core.Generators;
using Modsen.Portal.Booking.Requests.Workplaces.Core.Loaders;
using Modsen.Portal.Booking.Requests.Workplaces.Requests.DbQueries;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.Booking.Requests.Workplaces;

internal sealed class WorkplacesModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IBookingLoader, BookingLoader>();
        services.AddScoped<IBookingStartEndDateLoader, BookingStartEndDateLoader>();

        services.AddScoped<ICanCurrentUserDeleteBooking, CanCurrentUserDeleteBooking>();

        services.AddScoped<IRemoveBookingsRangeDbQuery, RemoveBookingsRangeDbQuery>();

        services.AddScoped<IBlockedBookingEntriesWithLastReservationDateComposer,
            BlockedBookingEntriesWithLastReservationDateComposer>();
        services.AddScoped<IDatesBetweenExcludingStartGenerator, DatesBetweenExcludingStartGenerator>();
        services.AddScoped<IReservationsLoader, ReservationsLoader>();
    }
}
