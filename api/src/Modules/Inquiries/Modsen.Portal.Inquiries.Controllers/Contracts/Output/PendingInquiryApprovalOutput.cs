using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Requests.RequestDtos;

namespace Modsen.Portal.Inquiries.Controllers.Contracts.Output;

internal sealed class PendingInquiryApprovalOutput
{
    public required int UserId { get; init; }
    public string? DepartmentId { get; init; }
    public required int InquiryId { get; init; }
    public required int InquiryTemplateId { get; init; }
    public required string InquiryTemplateName { get; init; }
    public required string UserFullName { get; init; }
    public InquiryType InquiryType { get; init; }
    public DateOnly DateFrom { get; init; }

    public static PendingInquiryApprovalOutput Of(PendingInquiryApprovalDto dto)
    {
        return new PendingInquiryApprovalOutput
        {
            UserId = dto.UserId,
            DepartmentId = dto.DepartmentId,
            InquiryId = dto.InquiryId,
            InquiryTemplateId = dto.InquiryTemplateId,
            InquiryTemplateName = dto.InquiryTemplateName,
            UserFullName = dto.UserFullName,
            InquiryType = dto.InquiryType,
            DateFrom = dto.DateFrom
        };
    }
}
