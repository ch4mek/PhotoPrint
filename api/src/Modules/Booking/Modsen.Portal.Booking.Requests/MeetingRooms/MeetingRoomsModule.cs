using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Booking.Requests.MeetingRooms.Core;
using Modsen.Portal.Booking.Requests.MeetingRooms.Core.BusinessRules;
using Modsen.Portal.Booking.Requests.MeetingRooms.Requests.Queries;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.Booking.Requests.MeetingRooms;

internal sealed class MeetingRoomsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddSingleton<ITimeZoneConverter, TimeZoneConverter>();
        services.AddSingleton<IMeetingRoomReservationDateGenerator, MeetingRoomReservationDateGenerator>();

        services.AddScoped<IGetMeetingRoomByIdDbQuery, GetMeetingRoomByIdDbQuery>();
        services.AddScoped<IGetOfficeByIdDbQuery, GetOfficeByIdDbQuery>();

        services.AddScoped<IMeetingRoomReservationConflictResolver, MeetingRoomReservationReservationConflictResolver>();
        services.AddScoped<IMeetingRoomReservationLoader, MeetingRoomReservationLoader>();
        services.AddScoped<IMeetingRoomEntryLoader, MeetingRoomEntryLoader>();
        services.AddScoped<IMeetingRoomReservationDateValidator, MeetingRoomReservationDateValidator>();

        services.AddScoped<IMeetingRoomAccessBusinessRule, MeetingRoomAccessBusinessRule>();
        services.AddScoped<IMeetingRoomEntryComposer, MeetingRoomEntryComposer>();
    }
}
