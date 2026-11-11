using System.Text.Json.Serialization;

namespace Modsen.Portal.ModsenApi.Adapter.ApiModels;

internal class CertificateApiModel
{
    [JsonPropertyName("certificateId")]
    public string? CertificateId { get; set; }

    [JsonPropertyName("receiptDate")]
    public DateTime? ReceiptDate { get; set; }

    [JsonPropertyName("recipient")]
    public string? Recipient { get; set; }

    [JsonPropertyName("recipientId")]
    public string? RecipientId { get; set; }

    [JsonPropertyName("technology")]
    public string? TechnologyName { get; set; }

    [JsonPropertyName("stack")]
    public string? JsStackName { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAtUtc { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime? UpdatedAtUtc { get; set; }

    [JsonPropertyName("certificateImageURL")]
    public string? CertificateImageUrl { get; set; }
}
