using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Inquiries.Requests.Calendar.Requests.Core;

namespace Modsen.Portal.Inquiries.Requests.Calendar;

internal sealed class CalendarModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<ICalendarExclusionValidator, CalendarExclusionValidator>();
    }
}
