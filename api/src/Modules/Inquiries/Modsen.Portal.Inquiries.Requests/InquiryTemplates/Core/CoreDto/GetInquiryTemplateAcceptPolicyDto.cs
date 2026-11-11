using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.CoreDto;

internal sealed class GetInquiryTemplateAcceptPolicyDto
{
    public int Id { get; set; }
    public int OrderWeight { get; set; }
    public InquiryAcceptPolicyScopes Scope { get; set; }
    public string? UserFullName { get; set; }
    public InquiryAcceptPolicyScopes? SubstituteScope { get; set; }
    public string? SubstituteUserFullName { get; set; }
}
