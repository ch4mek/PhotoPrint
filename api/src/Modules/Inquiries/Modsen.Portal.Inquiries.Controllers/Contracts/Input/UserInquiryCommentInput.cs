using Modsen.Portal.Inquiries.Requests.Comments.Requests.RequestsDto;

namespace Modsen.Portal.Inquiries.Controllers.Contracts.Input;

internal class UserInquiryCommentInput
{
    public required string Comment { get; set; }
    public bool? NotifyApprovers { get; set; }
    public IReadOnlyCollection<CreateInquiryCommentFileDto> Files { get; set; } = [];
}
