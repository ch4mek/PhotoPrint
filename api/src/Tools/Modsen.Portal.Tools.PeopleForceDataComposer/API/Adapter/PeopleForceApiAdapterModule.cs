using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Tools.PeopleForceDataComposer.API.Adapter.ApiClient;
using Modsen.Portal.Tools.PeopleForceDataComposer.API.Adapter.Contract;
using Modsen.Portal.Tools.PeopleForceDataComposer.API.Contract;

namespace Modsen.Portal.Tools.PeopleForceDataComposer.API.Adapter;

public sealed class PeopleForceApiAdapterModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        var appSettingsSection = Configuration.GetSection("AppSettings");
        var urlHolidayConfigKey = "PeopleForceSettings:ApiProdPeopleForceHolidaysUrl";
        var holidaysUrl = appSettingsSection.GetValue<string>(urlHolidayConfigKey);

        var urlLeaveConfigKey = "PeopleForceSettings:ApiProdPeopleForceLeavesUrl";
        var leavesUrl = appSettingsSection.GetValue<string>(urlLeaveConfigKey);

        var urlEmployeeConfigKey = "PeopleForceSettings:ApiProdPeopleForceEmployeesUrl";
        var employeesUrl = appSettingsSection.GetValue<string>(urlEmployeeConfigKey);

        var dataSyncUrl = "https://app.peopleforce.io";

        var options = new PeopleForceApiOptions
        {
            ApiKeyPeopleForce = appSettingsSection.GetValue<string>("PeopleForceSettings:ApiKeyPeopleForce"),
            ApiProdPeopleForceHolidaysUrl = holidaysUrl,
            ApiProdPeopleForceLeavesUrl = leavesUrl,
            ApiProdPeopleForceEmployeesUrl = employeesUrl
        };

        ArgumentException.ThrowIfNullOrEmpty(options.ApiProdPeopleForceHolidaysUrl);
        ArgumentException.ThrowIfNullOrEmpty(options.ApiProdPeopleForceLeavesUrl);

        services.AddScoped<IPeopleForceApiClient, PeopleForceApiClient>();
        services.AddScoped<IPeopleForceApiService, PeopleForceApiService>();
        services.AddHttpClient(PeopleForceApiConstants.PeopleForceHolidaysHttpClientName, httpClient =>
        {
            httpClient.DefaultRequestHeaders.Add(
                PeopleForceApiConstants.ApiKeyPeopleForceHeaderName,
                options.ApiKeyPeopleForce);
            httpClient.BaseAddress = new Uri(
                options.ApiProdPeopleForceHolidaysUrl!,
                UriKind.Absolute);
            httpClient.Timeout = TimeSpan.FromSeconds(60);
        });
        services.AddHttpClient(PeopleForceApiConstants.PeopleForceLeavesHttpClientName, httpClient =>
        {
            httpClient.DefaultRequestHeaders.Add(
                PeopleForceApiConstants.ApiKeyPeopleForceHeaderName,
                options.ApiKeyPeopleForce);
            httpClient.BaseAddress = new Uri(
                options.ApiProdPeopleForceLeavesUrl!,
                UriKind.Absolute);
            httpClient.Timeout = TimeSpan.FromSeconds(60);
        });
        services.AddHttpClient(PeopleForceApiConstants.PeopleForceEmployeesHttpClientName, httpClient =>
        {
            httpClient.DefaultRequestHeaders.Add(
                PeopleForceApiConstants.ApiKeyPeopleForceHeaderName,
                options.ApiKeyPeopleForce);
            httpClient.BaseAddress = new Uri(
                options.ApiProdPeopleForceEmployeesUrl!,
                UriKind.Absolute);
            httpClient.Timeout = TimeSpan.FromSeconds(60);
        });

        // to download all data
        services.AddHttpClient(PeopleForceApiConstants.PeopleForceHttpClientName, httpClient =>
        {
            httpClient.DefaultRequestHeaders.Add(
                PeopleForceApiConstants.ApiKeyPeopleForceHeaderName,
                options.ApiKeyPeopleForce);
            httpClient.BaseAddress = new Uri(
                dataSyncUrl!,
                UriKind.Absolute);
            httpClient.Timeout = TimeSpan.FromSeconds(60);
        });
    }
}
