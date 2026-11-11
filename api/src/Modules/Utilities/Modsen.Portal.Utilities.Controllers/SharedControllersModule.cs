using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Utilities.Controllers.Cache;

namespace Modsen.Portal.Utilities.Controllers;

public sealed class UtilitiesControllersModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IConfigurationCache, ConfigurationCache>();
    }
}
