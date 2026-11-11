using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.BcseApi.Contract;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.BcseApi.Adapter;

public sealed class BcseApiAdapterModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IBcseApiClient, BcseApiClient>();

        services.AddHttpClient(BcseApiConstants.BcseApiHttpClientName, (serviceProvider, httpClient) =>
        {
            httpClient.BaseAddress = ResolveBaseApiUrl(serviceProvider);
            httpClient.Timeout = TimeSpan.FromSeconds(60);
        }).AddStandardResilienceHandler();
    }

    private static Uri ResolveBaseApiUrl(IServiceProvider serviceProvider)
    {
        var baseApiUrl = serviceProvider
            .GetRequiredService<IConfiguration>()
            .GetValue<string>("AppSettings:BcseApi:BcseApiBaseUrl");

        ArgumentException.ThrowIfNullOrEmpty(baseApiUrl);

        return new Uri(baseApiUrl, UriKind.Absolute);
    }
}
