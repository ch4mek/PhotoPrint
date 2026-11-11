using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Api.Cors;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.Core.HostBuilder.HostModules;

internal sealed class CorsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddCorsPolicies(new CorsSettings
        {
            UseCors = true,
            UsePolicyWithName = PortalCorsPolicyNames.Default,
            Policies =
            [
                new CorsPolicySettings
                {
                    AllowAnyHeader = true,
                    AllowAnyMethod = true,
                    AllowedOrigins = ["localhost:7289"],
                    PolicyName = PortalCorsPolicyNames.Default
                }
            ]
        });
    }
}
