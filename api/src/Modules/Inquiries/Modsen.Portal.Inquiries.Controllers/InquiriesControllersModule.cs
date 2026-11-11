using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.HostBuilder.Jobs;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Inquiries.Controllers.Jobs;

namespace Modsen.Portal.Inquiries.Controllers;

public sealed class InquiriesControllersModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IRecurringJobScheduler, CreateAgreementTransactionsJobScheduler>();
        services.AddScoped<IRecurringJobScheduler, CreateDisposalTransactionsJobScheduler>();
    }

    public override IEnumerable<(string Name, AuthorizationPolicy Policy)> GetPolicies()
    {
        foreach (var policy in base.GetPolicies())
        {
            yield return policy;
        }

        yield return (PortalPolicyNames.ManageRequestsPolicy,
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == RequestsClaimTypeNames.ManageAllRequests.Name) ||
                        context.User.HasClaim(c => c.Type == RequestsClaimTypeNames.ManageRequests.Name)
                    )
                    .Build()
            );

        yield return (PortalPolicyNames.ManageAllRequestsPolicy,
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == RequestsClaimTypeNames.ManageAllRequests.Name)
                    )
                    .Build()
            );

        yield return (PortalPolicyNames.ViewOrManageRequestsPolicy,
            new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireAssertion(context =>
                    context.User.HasClaim(c => c.Type == RequestsClaimTypeNames.ManageAllRequests.Name)
                    || context.User.HasClaim(c => c.Type == RequestsClaimTypeNames.ManageRequests.Name)
                    || context.User.HasClaim(c => c.Type == RequestsClaimTypeNames.ViewAllRequests.Name)
                )
                .Build()
            );

        yield return (PortalPolicyNames.ManageAbsenceSettingsPolicy,
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == AbsencesClaimTypeNames.ManageAbsenceSettings.Name)
                    )
                    .Build()
            );

        yield return (PortalPolicyNames.ManageAssignsPolicy,
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == AbsencesClaimTypeNames.ManageAbsenceSettings.Name) ||
                        context.User.HasClaim(c => c.Type == HrClaimTypeNames.ManageHrModule.Name)
                    )
                    .Build()
            );
    }
}
