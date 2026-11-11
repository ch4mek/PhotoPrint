using Microsoft.AspNetCore.Authorization;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.KnowledgeBase.Controllers;

internal sealed class KnowledgeBaseControllersModule : RegistrationModule
{
    public override IEnumerable<(string Name, AuthorizationPolicy Policy)> GetPolicies()
    {
        foreach (var policy in base.GetPolicies())
        {
            yield return policy;
        }

        yield return (PortalPolicyNames.ManageArticlePolicy,
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireClaim(WikiClaimTypeNames.ManageArticle.Name)
                    .Build()
            );

        yield return (PortalPolicyNames.ManageGroupOrSubGroupPolicy,
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == WikiClaimTypeNames.ManageSubgroup.Name) ||
                        context.User.HasClaim(c => c.Type == WikiClaimTypeNames.ManageGroup.Name)
                    )
                    .Build()
            );

        yield return (PortalPolicyNames.ManageOrderPolicy,
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireClaim(WikiClaimTypeNames.ManageOrder.Name)
                    .Build()
            );

        yield return (PortalPolicyNames.ViewWikiPolicy,
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireClaim(WikiClaimTypeNames.ViewWiki.Name)
                    .Build()
            );

        yield return (PortalPolicyNames.ViewDiscountsPolicy,
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireClaim(DiscountsClaimTypeNames.ViewDiscounts.Name)
                    .Build()
            );

        yield return (PortalPolicyNames.ViewExportDiscountsPolicy,
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == DiscountsClaimTypeNames.ViewDiscounts.Name) ||
                        context.User.HasClaim(c => c.Type == DiscountsClaimTypeNames.ManageDiscountsExport.Name)
                    )
                    .Build()
            );

        yield return (PortalPolicyNames.ManageDiscountCategoryPolicy,
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireClaim(DiscountsClaimTypeNames.ManageDiscountCategory.Name)
                    .Build()
            );

        yield return (PortalPolicyNames.ManageDiscountCardPolicy,
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireClaim(DiscountsClaimTypeNames.ManageDiscountCard.Name)
                    .Build()
            );

        yield return (PortalPolicyNames.ManageDiscountOrderPolicy,
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireClaim(DiscountsClaimTypeNames.ManageDiscountOrder.Name)
                    .Build()
            );
    }
}
