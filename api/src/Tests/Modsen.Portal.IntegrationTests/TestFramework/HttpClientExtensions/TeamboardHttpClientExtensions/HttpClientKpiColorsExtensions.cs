using System.Net.Http.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.TeamBoard.Requests.Payload.KpiColors.Requests.RequestDto;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;

internal static class HttpClientKpiColorsExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static Task<List<KpiColorDto>> GetKpiColorsAsync(this HttpClient client)
    {
        return client.GetFromJsonAsync<List<KpiColorDto>>("api/kpi-colors", IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetKpiColorsAsHttpMessageAsync(this HttpClient client)
    {
        return client.GetAsync("api/kpi-colors", _cancellationToken);
    }

    public static Task<List<KpiColorDto>> UpdateKpiColorsAsync(this HttpClient client,
        UpdateKpiColorDto[] colors)
    {
        return client.PutAsJsonAsync<UpdateKpiColorDto[], List<KpiColorDto>>("api/kpi-colors", colors,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> UpdateKpiColorsAsHttpMessageAsync(this HttpClient client,
        UpdateKpiColorDto[] colors)
    {
        return client.PutAsJsonAsync("api/kpi-colors", colors, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }
}
