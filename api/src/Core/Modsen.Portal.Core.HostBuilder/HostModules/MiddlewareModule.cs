using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Api.Middleware;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.Core.HostBuilder.HostModules;

internal sealed class MiddlewareModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddSingleton<CorrelationIdMiddleware>();
        services.AddSingleton<CheckTokenRevokedMiddleware>();
    }
}
