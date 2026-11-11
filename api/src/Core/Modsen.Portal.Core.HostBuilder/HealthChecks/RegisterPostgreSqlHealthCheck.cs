using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Api.HealthChecks;
using Npgsql;

namespace Modsen.Portal.Core.HostBuilder.HealthChecks;

public sealed class RegisterPostgreSqlHealthCheck : IRegisterPortalHealthCheck
{
    private readonly string _serviceName;

    public RegisterPostgreSqlHealthCheck(string serviceName)
    {
        _serviceName = serviceName;
    }

    public void Register(IHealthChecksBuilder healthChecksBuilder)
    {
        healthChecksBuilder
            .AddNpgSql(
                dbDataSourceFactory: sp => sp.GetRequiredService<NpgsqlDataSource>(),
                name: $"{_serviceName} PostgreSQL Check",
                tags: ["data", PortalHealthCheckTagNames.Ready],
                healthQuery: "SELECT 1;",
                timeout: TimeSpan.FromSeconds(5));
    }
}
