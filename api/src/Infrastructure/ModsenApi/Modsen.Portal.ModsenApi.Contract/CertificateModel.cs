namespace Modsen.Portal.ModsenApi.Contract;

public class CertificateModel
{
    public string? CertificateId { get; set; }
    public DateTime? ReceiptDate { get; set; }
    public string? Recipient { get; set; }
    public string? RecipientId { get; set; }
    public string? Technology { get; set; }
    public string? Stack { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CertificateImageUrl { get; set; }
    public string? CertificateViewLink { get; set; }
}
