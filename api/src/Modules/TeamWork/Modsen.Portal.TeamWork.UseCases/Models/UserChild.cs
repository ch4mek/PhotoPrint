using System.Text.Json.Serialization;

namespace Modsen.Portal.TeamWork.UseCases.Models;

public sealed class UserChild : BaseChild
{
    [JsonPropertyName("_id")]
    public int Id { get; set; }

    public bool HasMembership { get; set; }
    public string? NameLowerCase { get; set; }
}
