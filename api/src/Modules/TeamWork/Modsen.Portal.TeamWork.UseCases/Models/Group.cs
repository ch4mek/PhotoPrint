using System.Text.Json.Serialization;

namespace Modsen.Portal.TeamWork.UseCases.Models;

public sealed class Group<T>
    where T : BaseChild
{
    public long Duration { get; set; }

    [JsonPropertyName("_id")]
    public string? Id { get; set; }

    public string? Name { get; set; }
    public IReadOnlyCollection<T> Children { get; set; } = [];
    public IReadOnlyCollection<object> Amounts { get; set; } = [];
}
