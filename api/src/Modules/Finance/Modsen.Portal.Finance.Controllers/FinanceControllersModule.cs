using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.HostBuilder.Jobs;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Finance.Controllers.Jobs;

namespace Modsen.Portal.Finance.Controllers;

public sealed class FinanceControllersModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddControllers();

        services.AddScoped<IRecurringJobScheduler, UpdateCurrentExchangeRatesJobScheduler>();
        services.AddScoped<IRecurringJobScheduler, BackFillExchangeRatesJobScheduler>();
    }

    public override IEnumerable<(string Name, AuthorizationPolicy Policy)> GetPolicies()
    {
        foreach (var policy in base.GetPolicies())
        {
            yield return policy;
        }

        yield return (PortalPolicyNames.ManageProjectRatesPolicy,
            new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireAssertion(context =>
                    context.User.HasClaim(c => c.Type == FinanceClaimTypeNames.ManageRates.Name
                                            || c.Type == FinanceClaimTypeNames.ManageAllRates.Name)
                )
                .Build()
            );

        yield return (PortalPolicyNames.ViewProjectRatesPolicy,
            new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireAssertion(context =>
                    context.User.HasClaim(c => c.Type == FinanceClaimTypeNames.ViewRates.Name
                                            || c.Type == FinanceClaimTypeNames.ViewAllRates.Name)
                )
                .Build()
            );

        yield return (PortalPolicyNames.ManageExpensesPolicy,
            new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireAssertion(context =>
                    context.User.HasClaim(c => c.Type == FinanceClaimTypeNames.ManageExpensesTab.Name)
                )
                .Build()
            );

        yield return (PortalPolicyNames.ViewManagementTabPolicy,
            new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireAssertion(context =>
                    context.User.HasClaim(c => c.Type == FinanceClaimTypeNames.ViewManagementTab.Name)
                )
                .Build()
            );
    }
}
