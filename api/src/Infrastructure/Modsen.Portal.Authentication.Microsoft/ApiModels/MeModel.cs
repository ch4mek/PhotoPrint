using System.Text.Json.Serialization;

namespace Modsen.Portal.Authentication.Microsoft.ApiModels;

internal sealed class MeModel
{
    [JsonPropertyName("givenName")]
    public string? GivenName { get; set; }

    [JsonPropertyName("surname")]
    public string? Surname { get; set; }

    [JsonPropertyName("mail")]
    public string? Email { get; set; }

    [JsonPropertyName("userPrincipalName")]
    public string? PrincipalName { get; set; }
}
