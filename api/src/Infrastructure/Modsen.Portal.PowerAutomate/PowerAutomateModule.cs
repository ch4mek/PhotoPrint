using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.PowerAutomate
{
    public sealed class PowerAutomateModule : RegistrationModule
    {
        public override void Load(IServiceCollection services)
        {
            services.AddScoped<IPowerAutomateService, PowerAutomateService>();
        }
    }
}
