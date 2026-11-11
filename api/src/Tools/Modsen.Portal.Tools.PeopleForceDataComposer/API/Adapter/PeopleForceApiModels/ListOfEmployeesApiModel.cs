using System.Text.Json.Serialization;

namespace Modsen.Portal.Tools.PeopleForceDataComposer.API.Adapter.PeopleForceApiModels;

internal sealed class ListOfEmployeesApiModel
{
    [JsonPropertyName("data")]
    public List<EmployeeApiModel> Employees { get; set; } = [];

    [JsonPropertyName("metadata")]
    public MetadataApiModel? Metadata { get; set; }
}
