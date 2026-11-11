using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.Builders;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.Loaders;

namespace Modsen.Portal.TimeTrack.UseCases.Dashboard
{
    internal sealed class DashboardModule : RegistrationModule
    {
        public override void Load(IServiceCollection services)
        {
            services.AddScoped<ITimesheetQueryBuilder, TimesheetQueryBuilder>();
            services.AddScoped<IDashboardOdpLoader, DashboardOdpLoader>();
            services.AddScoped<IDashboardOverviewLoader, DashboardOverviewLoader>();
            services.AddScoped<IDashboardUsersLoader, DashboardUsersLoader>();
        }
    }
}
