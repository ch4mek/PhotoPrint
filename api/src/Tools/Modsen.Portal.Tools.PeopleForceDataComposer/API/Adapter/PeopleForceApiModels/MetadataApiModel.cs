using System.Text.Json.Serialization;

namespace Modsen.Portal.Tools.PeopleForceDataComposer.API.Adapter.PeopleForceApiModels;

internal sealed class MetadataApiModel
{
    [JsonPropertyName("pagination")]
    public PaginationApiModel? Pagination { get; set; }
}
