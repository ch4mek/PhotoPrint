using Modsen.Portal.Inquiries.Requests.Comments.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Controllers.Contracts.Output;

internal class UserInquiryCommentOutput
{
    public int Id { get; set; }
    public string? Comment { get; set; }

    public InquiriesUserOutput? CreatedByUser { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public InquiriesUserOutput? UpdatedByUser { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public IReadOnlyCollection<InquiryCommentFileCoreDto> Files { get; set; } = [];

    public static UserInquiryCommentOutput Of(InquiryCommentCoreDto inquiryCommentDto)
    {
        return new UserInquiryCommentOutput
        {
            Id = inquiryCommentDto.Id,
            Comment = inquiryCommentDto.Comment,
            CreatedByUser = InquiriesUserOutput.Of(inquiryCommentDto.CreatedByUser),
            CreatedAtUtc = inquiryCommentDto.CreatedAtUtc,
            UpdatedByUser = InquiriesUserOutput.Of(inquiryCommentDto.UpdatedByUser),
            UpdatedAtUtc = inquiryCommentDto.UpdatedAtUtc,
            Files = inquiryCommentDto.Files
        };
    }
}
