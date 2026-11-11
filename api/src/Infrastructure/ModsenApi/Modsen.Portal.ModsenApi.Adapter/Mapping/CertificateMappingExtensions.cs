using System.Globalization;
using Modsen.Portal.ModsenApi.Adapter.ApiModels;
using Modsen.Portal.ModsenApi.Contract;

namespace Modsen.Portal.ModsenApi.Adapter.Mapping;

internal static class CertificateMappingExtensions
{
    internal static CertificateModel Map(this CertificateApiModel model, string? baseAddress = null)
    {
        var baseUri = baseAddress != null ? new Uri(baseAddress) : null;
        return new CertificateModel
        {
            CertificateId = model.CertificateId,
            CreatedAt = model.CreatedAtUtc,
            ReceiptDate = model.ReceiptDate,
            Recipient = model.Recipient,
            RecipientId = model.RecipientId,
            UpdatedAt = model.UpdatedAtUtc,
            Stack = model.JsStackName,
            Technology = model.TechnologyName,
            CertificateImageUrl= model.CertificateImageUrl,
            CertificateViewLink = baseAddress != null ? $"{baseUri?.Scheme}://{baseUri?.Host}/certificate-found?certificateId={model.CertificateId}&receiptDate={model.ReceiptDate}"
            : string.Empty
        };
    }

    internal static CreateCertificateApiModel MapWithStack(this CreateCertificateModel model, string dateFormat)
    {
        return new CreateCertificateApiModel
        {
            ReceiptDate = model.ReceiptDate.ToString(dateFormat, CultureInfo.InvariantCulture),
            Recipient = model.Recipient,
            RecipientId = model.RecipientId,
            Stack = model.Stack,
            Technology = model.Technology
        };
    }

    internal static CreateCertificateWithoutStackApiModel MapWithoutStack(this CreateCertificateModel model, string dateFormat)
    {
        return new CreateCertificateWithoutStackApiModel
        {
            ReceiptDate = model.ReceiptDate.ToString(dateFormat, CultureInfo.InvariantCulture),
            Recipient = model.Recipient,
            RecipientId = model.RecipientId,
            Technology = model.Technology
        };
    }
}
