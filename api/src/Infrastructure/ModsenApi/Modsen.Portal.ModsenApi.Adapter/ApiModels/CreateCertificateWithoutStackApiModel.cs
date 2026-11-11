using System.Text.Json.Serialization;

namespace Modsen.Portal.ModsenApi.Adapter.ApiModels;

internal class CreateCertificateWithoutStackApiModel
{
    [JsonPropertyName("receiptDate")]
    public string? ReceiptDate { get; set; }

    [JsonPropertyName("recipient")]
    public string? Recipient { get; set; }

    [JsonPropertyName("recipientId")]
    public string? RecipientId { get; set; }

    [JsonPropertyName("technology")]
    public string? Technology { get; set; }
}
