using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Jira.Client;
using Modsen.Portal.Jira.Contract;
using Modsen.Portal.Jira.Contract.Client;
using Modsen.Portal.Jira.Providers;

namespace Modsen.Portal.Jira;

public sealed class JiraModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddHttpClient(JiraConstants.JiraHttpClient, httpClient =>
        {
            httpClient.Timeout = TimeSpan.FromSeconds(15);
        });

        services.AddScoped<IJiraService, JiraService>();
        services.AddScoped<IJiraApiClientFactory, JiraApiClientFactory>();
        services.AddScoped<IJiraCloudAccessValidator, JiraCloudAccessValidator>();
        services.AddScoped<IJiraServerAccessValidator, JiraServerAccessValidator>();
        services.AddScoped<IJiraServerWorklogProvider, JiraServerWorklogProvider>();
        services.AddScoped<IJiraCloudWorklogProvider, JiraCloudWorklogProvider>();
    }
}
