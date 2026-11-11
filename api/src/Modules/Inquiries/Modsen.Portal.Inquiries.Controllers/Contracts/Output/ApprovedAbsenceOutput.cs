using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.Requests.Inquiries.Request.RequestDto;

namespace Modsen.Portal.Inquiries.Controllers.Contracts.Output;

internal sealed class ApprovedAbsenceOutput
{
    public required int UserId { get; init; }
    public required int InquiryId { get; init; }
    public required string InquiryTemplateName { get; init; }
    public required string UserFullName { get; init; }
    public InquiryType InquiryType { get; init; }
    public required string? UserPosition { get; init; }

    public static ApprovedAbsenceOutput Of(ApprovedAbsenceDto dto)
    {
        return new ApprovedAbsenceOutput
        {
            UserId = dto.UserId,
            InquiryId = dto.InquiryId,
            InquiryTemplateName = dto.InquiryTemplateName,
            UserFullName = dto.UserFullName,
            InquiryType = dto.InquiryType,
            UserPosition = dto.UserPosition,
        };
    }
}
