using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Booking.Requests.Booking.Core;
using Modsen.Portal.Booking.Requests.Booking.Core.BusinessRules;
using Modsen.Portal.Booking.Requests.Booking.Core.Queries;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.Booking.Requests.Booking;

internal sealed class BookingModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IAreCollisionsExistBusinessRule, AreCollisionsExistBusinessRule>();
        services.AddScoped<IIsCurrentUserInDepartmentBusinessRule, IsCurrentUserInDepartmentBusinessRule>();
        services.AddScoped<IIsCurrentUserInResourceGroupBusinessRule, IsCurrentUserInResourceGroupBusinessRule>();
        services.AddScoped<IIsAllowedToBookBusinessRule, IsAllowedToBookBusinessRule>();

        services.AddScoped<IGetUserByIdDbQuery, GetUserByIdDbQuery>();
        services.AddScoped<IGetWorkPlaceByIdDbQuery, GetWorkPlaceByIdDbQuery>();
        services.AddScoped<IGetWorkspaceAvailableStatusDbQuery, GetWorkspaceAvailableStatusDbQuery>();

        services.AddScoped<IBookingByDatesResolver, BookingByDatesResolver>();
        services.AddScoped<IBookingsWithoutUsersOwnReservationsCollisionsResolver, BookingsWithoutUsersOwnReservationsCollisionsResolver>();

        services.AddScoped<ICollisionsForDatesGenerator, CollisionsForDatesGenerator>();

        services.AddScoped<IBookingEntryLoader, BookingEntryLoader>();

        services.AddScoped<IBookingEntryComposer, BookingEntryComposer>();
    }
}
