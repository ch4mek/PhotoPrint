using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Api.Context;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.Core.HostBuilder.HostModules;

internal sealed class ContextModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IAuthenticatedUserContext, AuthenticatedUserContext>();
        services.AddScoped<ICorrelationContext, CorrelationContext>();
        services.AddScoped<IRequestExecutionContext, RequestExecutionContext>();
        services.AddSingleton<IHostEnvironmentContext, HostEnvironmentContext>();
    }
}
