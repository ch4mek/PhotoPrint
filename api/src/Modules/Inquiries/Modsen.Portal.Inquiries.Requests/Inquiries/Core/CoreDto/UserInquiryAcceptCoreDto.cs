using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.Requests.Dto;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;

internal class UserInquiryAcceptCoreDto
{
    public int Id { get; set; }
    public UserInquiryAcceptStatus Status { get; set; }

    public string? AcceptUserPosition { get; set; }
    public InquiriesUserDto? AcceptUser { get; set; }
    public InquiryAcceptPolicyScopes AcceptPolicyScope { get; set; }

    public InquiriesUserDto? AcceptedByUser { get; set; }
    public DateTime? AcceptedAtUtc { get; set; }

    public InquiriesUserDto? RejectedByUser { get; set; }
    public DateTime? RejectedAtUtc { get; set; }
}
