using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Controllers.Contracts.Output;

internal class InquiryTemplateAcceptPolicyOutput
{
    public int Id { get; set; }
    public int OrderWeight { get; set; }
    public InquiryAcceptPolicyScopes Scope { get; set; }
    public int? UserId { get; set; }
    public string? UserFullName { get; set; }
    public InquiryAcceptPolicyScopes? SubstituteScope { get; set; }
    public string? SubstituteUserFullName { get; set; }
    public int? SubstituteUserId { get; set; }

    public static InquiryTemplateAcceptPolicyOutput Of(GetInquiryTemplateAcceptPolicyDto dto)
    {
        return new InquiryTemplateAcceptPolicyOutput
        {
            Id = dto.Id,
            OrderWeight = dto.OrderWeight,
            Scope = dto.Scope,
            UserFullName = dto.UserFullName,
            SubstituteScope = dto.SubstituteScope,
            SubstituteUserFullName = dto.SubstituteUserFullName
        };
    }

    public static InquiryTemplateAcceptPolicyOutput Of(InquiryTemplateAcceptPolicyDto dto)
    {
        return new InquiryTemplateAcceptPolicyOutput
        {
            Id = dto.Id,
            OrderWeight = dto.OrderWeight,
            Scope = dto.PolicyScope,
            UserFullName = dto.UserFullName,
            SubstituteScope = dto.SubstituteScope,
            SubstituteUserFullName = dto.SubstituteUserFullName,
            SubstituteUserId = dto.SubstituteUserId,
            UserId = dto.UserId,
        };
    }
}
