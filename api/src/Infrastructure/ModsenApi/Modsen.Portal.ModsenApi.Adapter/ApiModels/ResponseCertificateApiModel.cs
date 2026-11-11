using System.Text.Json.Serialization;

namespace Modsen.Portal.ModsenApi.Adapter.ApiModels;

internal class ResponseCertificateApiModel
{
    [JsonPropertyName("success")]
    public bool IsSuccess { get; set; }

    [JsonPropertyName("data")]
    public CertificateApiModel? Certificate { get; set; }
}
