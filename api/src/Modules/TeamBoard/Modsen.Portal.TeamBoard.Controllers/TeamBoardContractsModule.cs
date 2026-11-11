using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.HostBuilder.Jobs;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TeamBoard.Controllers.Jobs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.Payslips.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers;

public sealed class TeamBoardContractsModule : RegistrationModule
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
                        "api/hard-skills/odata",
                        EdmModelBuilder.BuildWithoutExtraConfiguration<OdpHardSkill>());

                setup
                    .Filter()
                    .OrderBy()
                    .SetMaxTop(100)
                    .AddRouteComponents(
                        "api/soft-skills/odata",
                        EdmModelBuilder.BuildWithoutExtraConfiguration<OdpSoftSkill>());

                setup
                    .Filter()
                    .OrderBy()
                    .SetMaxTop(100)
                    .AddRouteComponents(
                        "api/specialization/odata",
                        EdmModelBuilder.BuildWithoutExtraConfiguration<OdpSpecializationTemplate>());

                setup
                    .Filter()
                    .OrderBy()
                    .SetMaxTop(100)
                    .AddRouteComponents(
                        "api/conversations/odata",
                        EdmModelBuilder.BuildWithoutExtraConfiguration<OdpConversation>());

                setup
                    .Filter()
                    .OrderBy()
                    .SetMaxTop(100)
                    .AddRouteComponents(
                        "api/payslips/odata",
                        EdmModelBuilder.BuildWithoutExtraConfiguration<OdpPayslip>());
            });

        services.AddScoped<IRecurringJobScheduler, ContractExpirationNoticeJobScheduler>();
    }
}
