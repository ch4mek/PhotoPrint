using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.HostBuilder.Jobs;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TimeTrack.Controllers.Jobs;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.OData;

namespace Modsen.Portal.TimeTrack.Controllers;

public sealed class TimeTrackContractsModule : RegistrationModule
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
                        "api/clients/odata",
                        EdmModelBuilder.BuildWithoutExtraConfiguration<OdpClient>())
                    .AddRouteComponents(
                        "api/projects/odata",
                        EdmModelBuilder.BuildWithoutExtraConfiguration<OdpProject>())
                    .AddRouteComponents(
                        "api/user-projects/odata",
                        EdmModelBuilder.BuildWithoutExtraConfiguration<OdpProjectUser>());
            });

        services.AddScoped<IRecurringJobScheduler, LogProjectsStateScheduler>();
        services.AddScoped<IRecurringJobScheduler, JiraTimesheetsImportScheduler>();
    }

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

        yield return (PortalPolicyNames.ViewHomePolicy,
            new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim(HomeClaimTypeNames.ViewHome.Name)
                .Build()
            );

        yield return (TimeTrackPolicyNames.ViewJiraImportPolicy,
            new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim(JiraClaimTypeNames.ViewJiraImport.Name)
                .Build()
            );
    }
}
