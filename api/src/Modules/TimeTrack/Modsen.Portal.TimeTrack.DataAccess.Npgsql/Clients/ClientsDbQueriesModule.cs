using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Clients;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Clients.GetWithTotalTrackedTime;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Clients;

public sealed class ClientsDbQueriesModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IGetClientsWithTotalTrackedTimeDbQuery, GetClientsWithTotalTrackedTimeDbQuery>();
        services.AddScoped<IGetResourceGroupIdsByClientDbQuery, GetResourceGroupIdsByClientDbQuery>();
        services.AddScoped<IGetPageableClientsDbQuery, GetPageableClientsDbQuery>();
        services.AddScoped<IGetClientNameByClientIdDbQuery, GetClientNameByClientIdDbQuery>();
        services.AddScoped<IGetClientNamesFromProjectsUserIsAssignedToDbQuery, GetClientNamesFromProjectsUserIsAssignedToDbQuery>();
    }
}
