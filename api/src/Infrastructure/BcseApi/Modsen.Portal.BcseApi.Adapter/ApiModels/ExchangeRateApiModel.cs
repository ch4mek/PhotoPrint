using System.Text.Json.Serialization;

namespace Modsen.Portal.BcseApi.Adapter.ApiModels;

internal class ExchangeRateApiModel
{
    [JsonPropertyName("toggle")]
    public string? Toggle { get; init; }

    [JsonPropertyName("include")]
    public string? Include { get; init; }

    [JsonPropertyName("currency")]
    public required string Currency { get; init; }

    [JsonPropertyName("rateCoef")]
    public required string RateCoef { get; init; }

    [JsonPropertyName("rateNB")]
    public required decimal RateNB { get; init; }
}
