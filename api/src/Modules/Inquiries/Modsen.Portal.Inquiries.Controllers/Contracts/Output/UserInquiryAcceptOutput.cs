using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Controllers.Contracts.Output;

internal class UserInquiryAcceptOutput
{
    public int Id { get; set; }
    public UserInquiryAcceptStatus Status { get; set; }

    public InquiryAcceptPolicyScopes AcceptPolicyScope { get; set; }
    public string? AcceptUserPosition { get; set; }
    public InquiriesUserOutput? AcceptUser { get; set; }

    public InquiriesUserOutput? AcceptedByUser { get; set; }
    public DateTime? AcceptedAtUtc { get; set; }

    public InquiriesUserOutput? RejectedByUser { get; set; }
    public DateTime? RejectedAtUtc { get; set; }

    public static UserInquiryAcceptOutput Of(UserInquiryAcceptCoreDto userInquiryAcceptCoreDto)
    {
        return new UserInquiryAcceptOutput
        {
            Id = userInquiryAcceptCoreDto.Id,
            Status = userInquiryAcceptCoreDto.Status,
            AcceptUser = InquiriesUserOutput.Of(userInquiryAcceptCoreDto.AcceptUser),
            AcceptedByUser = InquiriesUserOutput.Of(userInquiryAcceptCoreDto.AcceptedByUser),
            AcceptedAtUtc = userInquiryAcceptCoreDto.AcceptedAtUtc,
            RejectedByUser = InquiriesUserOutput.Of(userInquiryAcceptCoreDto.RejectedByUser),
            RejectedAtUtc = userInquiryAcceptCoreDto.RejectedAtUtc,
            AcceptUserPosition = userInquiryAcceptCoreDto.AcceptUserPosition,
            AcceptPolicyScope = userInquiryAcceptCoreDto.AcceptPolicyScope,
        };
    }
}
