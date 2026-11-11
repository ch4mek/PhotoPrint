using System.Text.Json.Serialization;

namespace Modsen.Portal.TeamWork.UseCases.Models;

public sealed class DepartmentChild : BaseChild
{
    [JsonPropertyName("_id")]
    public IReadOnlyCollection<string> Ids { get; set; } = [];
}
