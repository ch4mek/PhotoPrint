using System.Net.Http.Headers;
using System.Net.Mime;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Notification.DataAccess.Npgsql.Entities;
using Modsen.Portal.Notification.UseCases;

namespace Modsen.Portal.Notification.Controllers;

public sealed class NotificationContractsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddControllers()
            .AddOData(setup =>
            {
                setup.TimeZone = TimeZoneInfo.Utc;

                setup.Filter()
                    .OrderBy()
                    .SetMaxTop(100)
                    .AddRouteComponents("~/api/notifications/odata",
                        EdmModelBuilder.BuildWithoutExtraConfiguration<OdpNotification>());
            });

        services.AddScoped<ISignalRNotifier, SignalRNotifier>();
        services.AddScoped<IMicrosoftTeamsBotApiService, MicrosoftTeamsBotApiService>();

        services.AddHttpClient("MicrosoftTeamsBotApi", (sp, client) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var baseUrl = configuration["AppSettings:MicrosoftTeamsBotApi:MicrosoftTeamsBotApiUrl"];

            client.BaseAddress = new Uri(baseUrl!);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
        });

    }
}
