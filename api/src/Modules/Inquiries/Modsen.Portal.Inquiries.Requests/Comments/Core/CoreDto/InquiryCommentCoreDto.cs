using Modsen.Portal.Inquiries.Requests.Dto;

namespace Modsen.Portal.Inquiries.Requests.Comments.Core.CoreDto;

internal class InquiryCommentCoreDto
{
    public int Id { get; set; }
    public string? Comment { get; set; }

    public InquiriesUserDto? CreatedByUser { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public InquiriesUserDto? UpdatedByUser { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public IReadOnlyCollection<InquiryCommentFileCoreDto> Files { get; set; } = [];
}
