using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.BusinessRules;

namespace Modsen.Portal.Finance.Requests.ProjectRates;

internal sealed class ProjectRateModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<ICanFinanceUserEditProjectRatesBusinessRule, CanFinanceUserEditProjectRatesBusinessRule>();
        services.AddScoped<IIsProjectRateUniqueForDateBusinessRule, IsProjectRateUniqueForDateBusinessRule>();
    }
}
