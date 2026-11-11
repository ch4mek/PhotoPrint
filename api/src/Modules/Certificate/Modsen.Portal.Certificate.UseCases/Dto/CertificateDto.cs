using Modsen.Portal.ModsenApi.Contract;

namespace Modsen.Portal.Certificate.UseCases.Dto;

internal class CertificateDto
{
    public string? CertificateId { get; set; }
    public DateTime? ReceiptDate { get; set; }
    public string? Recipient { get; set; }
    public string? RecipientId { get; set; }
    public string? Technology { get; set; }
    public string? Stack { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CertificateImageURL { get; set; }
    public string? CertificateViewLink { get; set; }

    public static CertificateDto Of(CertificateModel source)
    {
        return new CertificateDto
        {
            CertificateId = source.CertificateId,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt,
            ReceiptDate = source.ReceiptDate,
            Recipient = source.Recipient,
            RecipientId = source.RecipientId,
            Stack = source.Stack,
            Technology = source.Technology,
            CertificateImageURL = source.CertificateImageUrl,
            CertificateViewLink = source.CertificateViewLink,
        };
    }
}
