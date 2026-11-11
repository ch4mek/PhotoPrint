using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Finance.Requests.ExchangeRates.Core.Composers;

namespace Modsen.Portal.Finance.Requests.ExchangeRates;

internal sealed class ExchangeRatesModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IExchangeRateComposer, ExchangeRateComposer>();
    }
}
