using System.Text.Json.Serialization;
using Modsen.Portal.Notification.UseCases.Core.Converters;

namespace Modsen.Portal.Notification.UseCases.Dto;

public class InquiryNotificationBodyDto
{
    public int Id { get; set; }
    public Uri Uri { get; set; } = null!;
    public string TemplateName { get; set; } = null!;
    public string InquiryType { get; set; } = null!;
    public string InquiryIconUri { get; set; } = null!;

    public UserNotificationBodyDto? User { get; set; }

    [JsonConverter(typeof(DateOnlyConverter))]
    public DateOnly DateFrom { get; set; }
    [JsonConverter(typeof(DateOnlyConverter))]
    public DateOnly DateTo { get; set; }

    public required string NotificationType { get; set; }
    public required string RecipientEmail { get; set; }
    public string Message { get; set; } = string.Empty;
}
