namespace Modsen.Portal.ModsenApi.Contract;

public class CreateCertificateModel
{
    public DateTime ReceiptDate { get; set; }

    public string? Recipient { get; set; }

    public string? RecipientId { get; set; }

    public string? Stack { get; set; }

    public string? Technology { get; set; }
}
