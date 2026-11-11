using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Agent.Contract;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.Agent;

public sealed class AgentModule : RegistrationModule
{
    private const string DirectlineHttpClientName = "Directline";
    private const string DirectlineRegionalHttpClientName = "DirectlineRegional";

    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IChatGptService, ChatGptService>();
        services.AddScoped<ICopilotService, CopilotService>();

        var appSettingsSection = Configuration.GetSection("CopilotAgent");
        var baseUrl = appSettingsSection.GetValue<string>("DirectLineUrl")
            ?? throw new InvalidOperationException("CopilotAgent:DirectLineUrl is not configured.");

        services.AddHttpClient(DirectlineHttpClientName, (provider, httpClient) =>
        {
            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.Timeout = TimeSpan.FromSeconds(60);
        });

        services.AddHttpClient(DirectlineRegionalHttpClientName, (provider, httpClient) =>
        {
            httpClient.Timeout = TimeSpan.FromSeconds(60);
        });
    }
}
