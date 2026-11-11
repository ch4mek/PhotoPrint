using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.TeamBoard.Requests.Users.Requests.RequestDto
{
    public class ImpermanentUserInfoDto
    {
        public DateTime? FuturePerformanceReviewDate { get; set; }
        public IReadOnlyCollection<UserInquiryDto>? OpenRequests { get; set; }
        public IReadOnlyCollection<UserBalanceDaysDto>? BalanceDays { get; set; }
    }

    public class UserInquiryDto
    {
        public required string InquiryTemplateName { get; set; }
        public InquiryType InquiryType { get; set; }
        public DateOnly DateFrom { get; set; }
        public DateOnly DateTo { get; set; }
        public UserInquiryStatus Status { get; set; }
    }
    
    public class UserBalanceDaysDto
    {
        public string? InquiryTemplateName { get; set; }
        public InquiryType InquiryType { get; set; }
        public decimal Balance { get; set; }
    }
}
