namespace Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Dto;

internal class InquiryUserNotifyDto
{
    public int UserInquiryId { get; set; }
    public int AcceptUserId { get; set; }
    public int? CreatedForUserId { get; set; }
}
