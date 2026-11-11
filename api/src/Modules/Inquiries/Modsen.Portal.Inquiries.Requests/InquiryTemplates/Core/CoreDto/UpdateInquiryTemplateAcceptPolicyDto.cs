using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.CoreDto;

internal sealed class UpdateInquiryTemplateAcceptPolicyDto
{
    public int OrderWeight { get; set; }
    public InquiryAcceptPolicyScopes Scope { get; set; }
    public int? UserId { get; set; }
    public InquiryAcceptPolicyScopes? SubstituteScope { get; set; }
    public int? SubstituteUserId { get; set; }
}
