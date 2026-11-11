using Microsoft.AspNetCore.Authorization;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.Merch.Controllers;

public sealed class MerchControllersModule : RegistrationModule
{
    public override IEnumerable<(string Name, AuthorizationPolicy Policy)> GetPolicies()
    {
        foreach (var policy in base.GetPolicies())
        {
            yield return policy;
        }

        yield return (
            PortalPolicyNames.ViewMerchPolicy,
            new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim(MerchClaimTypeNames.ViewMerch.Name)
                .Build()
        );

        yield return (
            PortalPolicyNames.ManageTransactionsPolicy,
            new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim(MerchClaimTypeNames.ManageMerchTransactions.Name)
                .Build()
        );

        yield return (
            PortalPolicyNames.ManageItemsAndKitsPolicy,
            new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim(MerchClaimTypeNames.ManageItemsAndKits.Name)
                .Build()
        );
    }
}
