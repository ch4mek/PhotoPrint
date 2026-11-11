using System.Text.Json.Serialization;

namespace Modsen.Portal.TeamWork.UseCases.Models;

public sealed class Total
{
    [JsonPropertyName("_id")]
    public string? Id { get; set; }

    public long TotalTime { get; set; }
    public int EntriesCount { get; set; }
    public object? TotalAmount { get; set; }
    public IReadOnlyCollection<object> Amounts { get; set; } = [];
}
