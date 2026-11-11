using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.HostBuilder.Jobs;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Identity.Controllers.Jobs;
using Modsen.Portal.Identity.Requests;
using Modsen.Portal.Identity.Requests.Core;

namespace Modsen.Portal.Identity.Controllers;

public sealed class IdentityControllersModule : RegistrationModule
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
                        "api/users/odata",
                        EdmModelBuilder.BuildWithoutExtraConfiguration<UserCoreDto>());
            });
        services.AddScoped<IIdentityContract, IdentityContract>();
        services.AddScoped<IRecurringJobScheduler, SyncDepartmentsJobScheduler>();
        services.AddScoped<IRecurringJobScheduler, SyncUsersJobScheduler>();
        services.AddScoped<IRecurringJobScheduler, DeleteInactiveGroupsJobScheduler>();
    }

    public override IEnumerable<(string Name, AuthorizationPolicy Policy)> GetPolicies()
    {
        foreach (var policy in base.GetPolicies())
        {
            yield return policy;
        }

        yield return (PortalPolicyNames.ChiefHeadGmSysAdminHRPolicyName,
            new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireAssertion(context =>
                    context.User.IsInRole(PortalRoleNames.Ceo) ||
                    context.User.IsInRole(PortalRoleNames.Cto) ||
                    context.User.IsInRole(PortalRoleNames.HeadOfDepartment) ||
                    context.User.IsInRole(PortalRoleNames.GroupManager) ||
                    context.User.IsInRole(PortalRoleNames.SystemAdministrator) ||
                    context.User.HasClaim(c => c.Type == HrClaimTypeNames.ManageHrModule.Name)
                )
                .Build()
        );

        yield return (PortalPolicyNames.ChiefHeadGmSysAdminHRAccountantCoordinatorPolicyName,
            new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireAssertion(context =>
                    context.User.IsInRole(PortalRoleNames.Ceo) ||
                    context.User.IsInRole(PortalRoleNames.Cto) ||
                    context.User.IsInRole(PortalRoleNames.HeadOfDepartment) ||
                    context.User.IsInRole(PortalRoleNames.GroupManager) ||
                    context.User.IsInRole(PortalRoleNames.SystemAdministrator) ||
                    context.User.HasClaim(c => c.Type == HrClaimTypeNames.ManageHrModule.Name) ||
                    context.User.HasClaim(c => c.Type == AccountantClaimTypeNames.ManageAccountant.Name) ||
                    context.User.HasClaim(c => c.Type == CoordinatorClaimTypeNames.ManageProjectCoordinator.Name)
                )
                .Build()
        );

        yield return (PortalPolicyNames.ChiefHeadGmHRPolicyName,
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireAssertion(context =>
                        context.User.IsInRole(PortalRoleNames.Ceo) ||
                        context.User.IsInRole(PortalRoleNames.Cto) ||
                        context.User.IsInRole(PortalRoleNames.HeadOfDepartment) ||
                        context.User.IsInRole(PortalRoleNames.GroupManager) ||
                        context.User.HasClaim(c => c.Type == HrClaimTypeNames.ManageHrModule.Name)
                    )
                    .Build()
            );

        yield return (PortalPolicyNames.ChiefAndHRPolicyName,
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireAssertion(context =>
                        context.User.IsInRole(PortalRoleNames.Ceo) ||
                        context.User.IsInRole(PortalRoleNames.Cto) ||
                        context.User.HasClaim(c => c.Type == HrClaimTypeNames.ManageHrModule.Name)
                    )
                    .Build()
            );

        yield return (PortalPolicyNames.ChiefHeadHrPolicyName,
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireAssertion(context =>
                        context.User.IsInRole(PortalRoleNames.Ceo) ||
                        context.User.IsInRole(PortalRoleNames.Cto) ||
                        context.User.IsInRole(PortalRoleNames.HeadOfDepartment) ||
                        context.User.HasClaim(c => c.Type == HrClaimTypeNames.ManageHrModule.Name)
                    )
                    .Build()
            );

        yield return (PortalPolicyNames.ChiefHeadGmSysAdminOrProjectExitsPolicyName,
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireAssertion(context =>
                        context.User.IsInRole(PortalRoleNames.Ceo) ||
                        context.User.IsInRole(PortalRoleNames.Cto) ||
                        context.User.IsInRole(PortalRoleNames.HeadOfDepartment) ||
                        context.User.IsInRole(PortalRoleNames.GroupManager) ||
                        context.User.IsInRole(PortalRoleNames.SystemAdministrator) ||
                        context.User.HasClaim(c => c.Type == StaffingClaimTypeNames.ViewProjectExits.Name) ||
                        context.User.HasClaim(c => c.Type == StaffingClaimTypeNames.ManageAllProjectExits.Name)
                    )
                    .Build()
            );
    }
}
