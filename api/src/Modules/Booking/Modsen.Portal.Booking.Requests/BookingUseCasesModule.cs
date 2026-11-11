using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Booking.Contract;
using Modsen.Portal.Booking.Requests.Booking;
using Modsen.Portal.Booking.Requests.BookingPortalSettings;
using Modsen.Portal.Booking.Requests.Consumers;
using Modsen.Portal.Booking.Requests.Core.DbQueries;
using Modsen.Portal.Booking.Requests.MeetingRooms;
using Modsen.Portal.Booking.Requests.Services;
using Modsen.Portal.Booking.Requests.Services.Contracts;
using Modsen.Portal.Booking.Requests.Workplaces;
using Modsen.Portal.Booking.Requests.Workplaces.Core.BusinessRules;
using Modsen.Portal.Booking.Requests.Workplaces.Core.Loaders;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.Booking.Requests;

public sealed class BookingUseCasesModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        AddModule<MeetingRoomsModule>();
        AddModule<WorkplacesModule>();
        AddModule<BookingModule>();

        base.Load(services);

        services.AddScoped<IBookingClockService, BookingClockService>();
        services.AddScoped<IBookingContract, BookingContract>();
        services.AddScoped<IReservationsLoader, ReservationsLoader>();
        services.AddScoped<ICanBookingUserBeDeactivatedBusinessRule, CanBookingUserBeDeactivatedBusinessRule>();
        services.AddScoped<IBookingPortalSettingsAccessor, BookingPortalSettingsAccessor>();
        services.AddScoped<ICreateOrUpdateUserDbQuery, CreateOrUpdateUserDbQuery>();
    }


    public override IEnumerable<IConsumerConfigurationRegistration> GetConsumerRegistrationConfigurations()
    {
        yield return new UserActivatedEventConsumer.RegistrationDefinition();
        yield return new UserCreatedEventConsumer.RegistrationDefinition();
        yield return new UserDeactivatedEventConsumer.RegistrationDefinition();
        yield return new UserSyncEventConsumer.RegistrationDefinition();
        yield return new UserUpdatedEventConsumer.RegistrationDefinition();
    }
}
