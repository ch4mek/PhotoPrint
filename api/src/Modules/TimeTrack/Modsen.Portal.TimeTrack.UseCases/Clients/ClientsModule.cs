using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TimeTrack.UseCases.Clients.Core.Loaders;

namespace Modsen.Portal.TimeTrack.UseCases.Clients;

internal sealed class ClientsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IClientLookupLoader, ClientLookupLoader>();
    }
}
