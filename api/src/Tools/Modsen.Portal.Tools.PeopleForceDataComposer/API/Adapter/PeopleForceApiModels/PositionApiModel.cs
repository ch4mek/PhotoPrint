using System.Text.Json.Serialization;

namespace Modsen.Portal.Tools.PeopleForceDataComposer.API.Adapter.PeopleForceApiModels;

internal sealed class PositionApiModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
