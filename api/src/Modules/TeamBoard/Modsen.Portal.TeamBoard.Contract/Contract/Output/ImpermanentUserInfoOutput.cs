using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.TeamBoard.Contract.Contract.Output
{
    public class ImpermanentUserInfoOutput
    {
        public DateTime? FuturePerformanceReviewDate { get; set; }
        public IReadOnlyCollection<UserInquiryOutput>? OpenRequests { get; set; }
        public IReadOnlyCollection<UserBalanceDaysOutput>? BalanceDays { get; set; }
    }

    public class UserInquiryOutput
    {
        public string? InquiryTemplateName { get; set; }
        public InquiryType InquiryType { get; set; }
        public DateOnly DateFrom { get; set; }
        public DateOnly DateTo { get; set; }
        public UserInquiryStatus Status { get; set; }
    }

    public class UserBalanceDaysOutput
    {
        public string? InquiryTemplateName { get; set; }
        public InquiryType InquiryType { get; set; }
        public decimal Balance { get; set; }
    }
}
