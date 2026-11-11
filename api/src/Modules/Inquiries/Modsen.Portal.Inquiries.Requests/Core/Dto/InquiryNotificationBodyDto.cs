using System.Globalization;
using System.Text.Json.Serialization;

namespace Modsen.Portal.Inquiries.Requests.Core.Dto;

public class InquiryNotificationBodyDto
{
    public int Id { get; set; }
    public Uri Uri { get; set; } = null!;
    public string TemplateName { get; set; } = null!;
    public string InquiryType { get; set; } = null!;
    public string InquiryIconUri { get; set; } = null!;

    public UserNotificationBodyDto? User { get; set; }

    [JsonIgnore] public DateOnly DateFrom { get; set; }
    [JsonPropertyName("dateFrom")]
    public string DateFromFormatted => DateFrom.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);

    [JsonIgnore] public DateOnly DateTo { get; set; }
    [JsonPropertyName("dateTo")]
    public string DateToFormatted => DateTo.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
    public required string NotificationType { get; set; }
    public required string RecipientEmail { get; set; }
    public string Message { get; set; } = string.Empty;
}
