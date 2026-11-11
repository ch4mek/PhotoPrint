using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Booking.Controllers.Jobs;
using Modsen.Portal.Core.HostBuilder.Jobs;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.Booking.Controllers;

internal sealed class BookingControllersModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IRecurringJobScheduler, UpdateReservationsForBlockedWorkplacesJobScheduler>();
        services.AddScoped<IRecurringJobScheduler, UpdateReservationsForRepetitiveMeetingRoomJobScheduler>();
    }
}
