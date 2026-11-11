using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Api.HealthChecks;

namespace Modsen.Portal.Core.HostBuilder.HealthChecks;

public sealed class RegisterRedisHealthCheck : IRegisterPortalHealthCheck
{
    private readonly string _serviceName;
    private readonly IConfiguration _configuration;

    public RegisterRedisHealthCheck(string serviceName, IConfiguration configuration)
    {
        _serviceName = serviceName;
        _configuration = configuration;
    }

    public void Register(IHealthChecksBuilder healthChecksBuilder)
    {
        if (_configuration.GetValue<string>("CacheStore") == "Redis")
        {
            healthChecksBuilder
                .AddRedis(connectionStringFactory:
                    serviceProvider =>
                    {
                        var config = serviceProvider.GetRequiredService<IConfiguration>();
                        return config.GetConnectionString("Redis")!;
                    },
                    name: $"{_serviceName} Redis Check",
                    tags: ["data", "redis-cache", PortalHealthCheckTagNames.Ready],
                    timeout: TimeSpan.FromSeconds(5));
        }
    }
}
