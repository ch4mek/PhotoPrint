using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TimeTrack.UseCases.Reports.Core;

namespace Modsen.Portal.TimeTrack.UseCases.Reports;

internal sealed class ReportsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IProjectsWithTotalTrackedTimeResolver, ProjectsWithTotalTrackedTimeResolver>();
        services.AddScoped<IClientsWithTotalTrackedTimeResolver, ClientsWithTotalTrackedTimeResolver>();
    }
}
