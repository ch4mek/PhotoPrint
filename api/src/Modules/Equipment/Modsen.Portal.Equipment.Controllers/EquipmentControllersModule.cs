using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Equipment.DataAccess.Npgsql.OData;

namespace Modsen.Portal.Equipment.Controllers;

public sealed class EquipmentControllersModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddControllers()
            .AddOData(setup =>
            {
                setup.TimeZone = TimeZoneInfo.Utc;

                setup
                    .Filter()
                    .OrderBy()
                    .SetMaxTop(100)
                    .AddRouteComponents(
                        "api/equipment/odata",
                        EdmModelBuilder.BuildWithoutExtraConfiguration<OdpEquipment>());
            });
    }

    public override IEnumerable<(string Name, AuthorizationPolicy Policy)> GetPolicies()
    {
        foreach (var policy in base.GetPolicies())
        {
            yield return policy;
        }

        yield return (PortalPolicyNames.ManageAllEquipmentOrChiefOrSysAdminPolicy,
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireAssertion(context =>
                        context.User.IsInRole(PortalRoleNames.Ceo) ||
                        context.User.IsInRole(PortalRoleNames.Cto) ||
                        context.User.IsInRole(PortalRoleNames.SystemAdministrator) ||
                        context.User.HasClaim(c => c.Type == EquipmentClaimTypeNames.ManageAllEquipment.Name)
                    )
                    .Build()
            );

        yield return (PortalPolicyNames.ManageAllEquipmentOrChiefOrHeadOrGmOrSysAdminPolicy,
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireAssertion(context =>
                        context.User.IsInRole(PortalRoleNames.Ceo) ||
                        context.User.IsInRole(PortalRoleNames.Cto) ||
                        context.User.IsInRole(PortalRoleNames.HeadOfDepartment) ||
                        context.User.IsInRole(PortalRoleNames.GroupManager) ||
                        context.User.IsInRole(PortalRoleNames.SystemAdministrator) ||
                        context.User.HasClaim(c => c.Type == EquipmentClaimTypeNames.ManageAllEquipment.Name)
                    )
                    .Build()
            );
    }
}
