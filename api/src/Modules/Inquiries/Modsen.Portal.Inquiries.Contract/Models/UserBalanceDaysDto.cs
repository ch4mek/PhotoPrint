using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.Inquiries.Contract.Models
{
    public class UserBalanceDaysDto
    {
        public int UserId { get; set; }
        public required string InquiryTemplateName { get; set; }
        public InquiryType InquiryType { get; set; }
        public decimal Balance { get; set; }
    }
}
