using System.Net.Http.Json;
using Modsen.Portal.Utilities.Controllers.Configuration;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

internal static class HttpClientConfigurationExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static Task<ConfigurationOutput?> GetConfiguration(this HttpClient client)
    {
        return client.GetFromJsonAsync<ConfigurationOutput>(
            "api/Configuration",
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }
}
