using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Request.RequestDto;

internal sealed class ApprovedAbsenceDto
{
    public required int UserId { get; init; }
    public required int InquiryId { get; init; }
    public required string InquiryTemplateName { get; init; }
    public required string UserFullName { get; init; }
    public InquiryType InquiryType { get; init; }
    public required string? UserPosition { get; init; }
}
