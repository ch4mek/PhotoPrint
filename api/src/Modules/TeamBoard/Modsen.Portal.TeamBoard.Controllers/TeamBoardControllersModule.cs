using Microsoft.AspNetCore.Authorization;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.TeamBoard.Controllers
{
    internal sealed class TeamBoardControllersModule : RegistrationModule
    {
        public override IEnumerable<(string Name, AuthorizationPolicy Policy)> GetPolicies()
        {
            foreach (var policy in base.GetPolicies())
            {
                yield return policy;
            }

            yield return (PortalPolicyNames.ViewDashboardPolicy,
                    new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .RequireClaim(DashboardClaimTypeNames.ViewDashboard.Name)
                        .Build()
                );

            yield return (PortalPolicyNames.ManageInterviewsPolicy,
                    new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .RequireAssertion(context =>
                            context.User.HasClaim(c => c.Type == StaffingClaimTypeNames.ManageInterviews.Name)
                            || context.User.HasClaim(c => c.Type == StaffingClaimTypeNames.ManageAllInterviews.Name))
                        .Build()
                );

            yield return (PortalPolicyNames.ViewOrManageProjectExitsPolicy,
                    new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .RequireAssertion(context =>
                            context.User.HasClaim(c => c.Type == StaffingClaimTypeNames.ViewProjectExits.Name)
                            || context.User.HasClaim(c => c.Type == StaffingClaimTypeNames.ManageAllProjectExits.Name))
                        .Build()
                );

            yield return (PortalPolicyNames.ManageProjectExitsPolicy,
                    new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .RequireClaim(StaffingClaimTypeNames.ManageAllProjectExits.Name)
                        .Build()
                );

            yield return (PortalPolicyNames.ViewHomePolicy,
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireClaim(HomeClaimTypeNames.ViewHome.Name)
                    .Build()
                );

            yield return (PortalPolicyNames.ChiefHeadGmEmployeeSysAdminAndViewHomeOnlyPolicy,
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireAssertion(context =>
                        (context.User.IsInRole(PortalRoleNames.Ceo)
                        || context.User.IsInRole(PortalRoleNames.Cto)
                        || context.User.IsInRole(PortalRoleNames.HeadOfDepartment)
                        || context.User.IsInRole(PortalRoleNames.GroupManager)
                        || context.User.IsInRole(PortalRoleNames.SystemAdministrator)
                        || context.User.IsInRole(PortalRoleNames.Employee))
                        && context.User.HasClaim(c => c.Type == HomeClaimTypeNames.ViewHome.Name))
                    .Build()
                );

            yield return (PortalPolicyNames.ChiefHeadGmSysAdminHRAccountantPolicyName,
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireAssertion(context =>
                        context.User.IsInRole(PortalRoleNames.Ceo)
                        || context.User.IsInRole(PortalRoleNames.Cto)
                        || context.User.IsInRole(PortalRoleNames.HeadOfDepartment)
                        || context.User.IsInRole(PortalRoleNames.GroupManager)
                        || context.User.IsInRole(PortalRoleNames.SystemAdministrator)
                        || context.User.HasClaim(c => c.Type == HrClaimTypeNames.ManageHrModule.Name)
                        || context.User.HasClaim(c => c.Type == AccountantClaimTypeNames.ManageAccountant.Name))
                    .Build()
                );

            yield return (PortalPolicyNames.ChiefHeadGmHRAccountantPolicyName,
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireAssertion(context =>
                        context.User.IsInRole(PortalRoleNames.Ceo)
                        || context.User.IsInRole(PortalRoleNames.Cto)
                        || context.User.IsInRole(PortalRoleNames.HeadOfDepartment)
                        || context.User.IsInRole(PortalRoleNames.GroupManager)
                        || context.User.HasClaim(c => c.Type == HrClaimTypeNames.ManageHrModule.Name)
                        || context.User.HasClaim(c => c.Type == AccountantClaimTypeNames.ManageAccountant.Name))
                    .Build()
                );

            yield return (PortalPolicyNames.ViewLabBoardPolicy,
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireAssertion(context =>
                            context.User.HasClaim(c => c.Type == LabBoardClaimTypeNames.ManageInterns.Name)
                            || context.User.HasClaim(c => c.Type == LabBoardClaimTypeNames.ManageAllInterns.Name))
                    .Build()
                );

            yield return (PortalPolicyNames.ViewInternshipPolicy,
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireAssertion(context =>
                            context.User.HasClaim(c => c.Type == LabBoardClaimTypeNames.ManageInterns.Name)
                            || context.User.HasClaim(c => c.Type == LabBoardClaimTypeNames.ManageAllInterns.Name)
                            || context.User.HasClaim(c => c.Type == HrClaimTypeNames.ManageHrModule.Name))
                    .Build()
                );

            yield return (PortalPolicyNames.ChiefHeadGmHRManageInternsPolicyName,
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireAssertion(context =>
                        context.User.IsInRole(PortalRoleNames.Ceo) ||
                        context.User.IsInRole(PortalRoleNames.Cto) ||
                        context.User.IsInRole(PortalRoleNames.HeadOfDepartment) ||
                        context.User.IsInRole(PortalRoleNames.GroupManager) ||
                        context.User.HasClaim(c => c.Type == HrClaimTypeNames.ManageHrModule.Name) ||
                        context.User.HasClaim(c => c.Type == LabBoardClaimTypeNames.ManageInterns.Name) ||
                        context.User.HasClaim(c => c.Type == LabBoardClaimTypeNames.ManageAllInterns.Name)
                    )
                    .Build()
                );
        }
    }
}
