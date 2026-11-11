using Microsoft.Extensions.Hosting;
using Modsen.Portal.Core.Context;

namespace Modsen.Portal.Core.Api.Context;

public sealed class HostEnvironmentContext : IHostEnvironmentContext
{
    private readonly IHostEnvironment _hostEnvironment;

    public HostEnvironmentContext(IHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
    }

    public bool IsProductionEnvironment()
    {
        return _hostEnvironment.IsProduction();
    }

    public bool IsDevelopmentOrQaEnvironment()
    {
        return _hostEnvironment.IsDevelopment() || _hostEnvironment.IsEnvironment(PortalEnvironmentNames.Qa);
    }

    public bool IsStagingEnvironment()
    {
        return _hostEnvironment.IsStaging();
    }

    public bool IsDevelopmentEnvironment()
    {
        return _hostEnvironment.IsDevelopment();
    }

    public bool IsQaEnvironment()
    {
        return _hostEnvironment.IsEnvironment(PortalEnvironmentNames.Qa);
    }

    public bool IsFunctionalTestEnvironment()
    {
        return _hostEnvironment.IsEnvironment(PortalEnvironmentNames.FunctionalTest);
    }

    public bool IsIntegrationTestEnvironment()
    {
        return _hostEnvironment.IsEnvironment(PortalEnvironmentNames.IntegrationTest);
    }
}
