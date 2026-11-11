using Microsoft.Extensions.DependencyInjection;

namespace Modsen.Portal.Core.HostBuilder.HealthChecks;

public interface IRegisterPortalHealthCheck
{
    void Register(IHealthChecksBuilder healthChecksBuilder);
}
