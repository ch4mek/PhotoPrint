using System.Text.Json.Serialization;

namespace Modsen.Portal.Tools.PeopleForceDataComposer.API.Adapter.PeopleForceApiModels;

internal sealed class PaginationApiModel
{
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("pages")]
    public int Pages { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("items")]
    public int Items { get; set; }
}
