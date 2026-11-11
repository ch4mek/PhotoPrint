using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.Core.UseCases;

public sealed class ConnectionModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddSingleton<PortalDbDataSource>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            return new DataSourceFactory(configuration).Create();
        });

        services.AddScoped<IConnectionFactory, ConnectionFactory>();
    }
}
