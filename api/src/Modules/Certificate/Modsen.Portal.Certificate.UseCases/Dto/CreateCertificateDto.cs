using Modsen.Portal.Core.Certificate;
using Modsen.Portal.ModsenApi.Contract;

namespace Modsen.Portal.Certificate.UseCases.Dto;

internal class CreateCertificateDto
{
    public DateTime ReceiptDate { get; set; }

    public string? Recipient { get; set; }

    public string? RecipientId { get; set; }

    public JsStackType? Stack { get; set; }

    public required string Technology { get; set; }


    public static CreateCertificateModel ToModel(CreateCertificateDto source)
    {
        return new CreateCertificateModel()
        {
            ReceiptDate = source.ReceiptDate,
            Recipient = source.Recipient,
            RecipientId = source.RecipientId,
            Technology = source.Technology,
            Stack = source.Stack is null or JsStackType.None
                ? null
                : Enum.GetName(typeof(JsStackType), source.Stack)
        };
    }
}
