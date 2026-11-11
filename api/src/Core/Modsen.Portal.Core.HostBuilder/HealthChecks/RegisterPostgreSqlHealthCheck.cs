using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Api.HealthChecks;
using Modsen.Portal.Core.Database;
using MySqlConnector;

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
            .AddCheck<MySqlHealthCheck>(
                $"{_serviceName} MySQL Check",
                tags: ["data", PortalHealthCheckTagNames.Ready],
                timeout: TimeSpan.FromSeconds(5));
    }

    private sealed class MySqlHealthCheck(PortalDbDataSource dataSource)
        : Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck
    {
        public async Task<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult> CheckHealthAsync(
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await using var connection = dataSource.CreateConnection();
                await connection.OpenAsync(cancellationToken);
                await using var command = connection.CreateCommand();
                command.CommandText = "SELECT 1;";
                await command.ExecuteScalarAsync(cancellationToken);
                return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return new Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult(
                    context.Registration.FailureStatus, "MySQL недоступна", ex);
            }
        }
    }
}
