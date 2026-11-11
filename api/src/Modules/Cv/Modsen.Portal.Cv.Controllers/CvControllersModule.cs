using Microsoft.AspNetCore.Authorization;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.Cv.Controllers;

public sealed class CvControllersModule : RegistrationModule
{
    public override IEnumerable<(string Name, AuthorizationPolicy Policy)> GetPolicies()
    {
        foreach (var policy in base.GetPolicies())
        {
            yield return policy;
        }

        yield return (PortalPolicyNames.ManageCvsPolicy,
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == CvsClaimTypeNames.ManageCvs.Name)
                    )
                    .Build()
            );
    }
}
