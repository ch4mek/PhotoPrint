using System.Text.Json.Serialization;

namespace Modsen.Portal.ModsenApi.Adapter.ApiModels;

internal class ListOfCertificatesApiModel
{
    [JsonPropertyName("success")]
    public bool IsSuccess { get; set; }

    [JsonPropertyName("data")]
    public List<CertificateApiModel> Certificates { get; set; } = [];
}
