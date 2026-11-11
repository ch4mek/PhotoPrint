using System.Text.Json.Serialization;

namespace Modsen.Portal.BcseApi.Adapter.ApiModels;

internal class ExchangeRateApiResponse
{
    [JsonPropertyName("rates")]
    public List<ExchangeRateApiModel> Rates { get; init; } = [];
}
