using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.Inquiries.Contract.Models;

public class UserInquiryDto
{
    public int Id { get; set; }
    public int UserId { get; set; }    
    public required string InquiryTemplateName { get; set; }
    public InquiryType InquiryType { get; set; }
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
    public UserInquiryStatus Status { get; set; }
}
