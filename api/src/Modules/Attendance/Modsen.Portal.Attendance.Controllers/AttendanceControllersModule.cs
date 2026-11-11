using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Attendance.Controllers.Jobs;
using Modsen.Portal.Core.HostBuilder.Jobs;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.Attendance.Controllers;

public class AttendanceControllersModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IRecurringJobScheduler, SyncAttendancesJobScheduler>();
    }
}
