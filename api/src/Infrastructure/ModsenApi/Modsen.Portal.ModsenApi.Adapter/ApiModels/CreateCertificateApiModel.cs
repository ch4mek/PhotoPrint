using System.Text.Json.Serialization;

namespace Modsen.Portal.ModsenApi.Adapter.ApiModels;

internal class CreateCertificateApiModel : CreateCertificateWithoutStackApiModel
{
    [JsonPropertyName("stack")]
    public string? Stack { get; set; }
}
