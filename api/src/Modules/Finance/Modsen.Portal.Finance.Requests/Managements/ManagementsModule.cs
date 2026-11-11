using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Finance.Requests.Managements.Core.Calculators;

namespace Modsen.Portal.Finance.Requests.Managements;

internal sealed class ManagementsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IManagementsCalculator, ManagementsCalculator>();
    }
}
