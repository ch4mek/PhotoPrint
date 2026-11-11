using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.ModsenApi.Contract;

namespace Modsen.Portal.ModsenApi.Adapter;

public sealed class ModsenApiAdapterModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IModsenApiCertificatesClient, ModsenApiCertificatesClient>();

        services.AddHttpClient(ModsenApiConstants.CertificatesHealthCheckHttpClientName, (serviceProvider, httpClient) =>
        {
            httpClient.DefaultRequestHeaders.Add(ModsenApiConstants.ApiKeyCertificateHeaderName, ResolveApiKey(serviceProvider));
            httpClient.BaseAddress = new Uri(ResolveApiUrl(serviceProvider), UriKind.Absolute);
        }).AddStandardResilienceHandler();

        services.AddHttpClient(ModsenApiConstants.CertificatesHttpClientName, (serviceProvider, httpClient) =>
        {
            httpClient.DefaultRequestHeaders.Add(ModsenApiConstants.ApiKeyCertificateHeaderName, ResolveApiKey(serviceProvider));
            httpClient.BaseAddress = new Uri(ResolveApiUrl(serviceProvider), UriKind.Absolute);
            httpClient.Timeout = TimeSpan.FromSeconds(60);
        }).AddStandardResilienceHandler();
    }

    private static string ResolveApiUrl(IServiceProvider serviceProvider)
    {
        var settingsKey = serviceProvider.GetRequiredService<IHostEnvironment>().IsProduction()
            ? "AppSettings:CertificateSettings:ApiProdCertificateUrl"
            : "AppSettings:CertificateSettings:ApiCertificateUrl";
        var apiUrl = serviceProvider.GetRequiredService<IConfiguration>().GetValue<string>(settingsKey);

        ArgumentException.ThrowIfNullOrEmpty(apiUrl);

        return apiUrl;
    }

    private static string ResolveApiKey(IServiceProvider serviceProvider)
    {
        var apiKey = serviceProvider.GetRequiredService<IConfiguration>()
            .GetValue<string>("AppSettings:CertificateSettings:ApiKeyCertificate");

        ArgumentException.ThrowIfNullOrEmpty(apiKey);

        return apiKey;
    }
}
