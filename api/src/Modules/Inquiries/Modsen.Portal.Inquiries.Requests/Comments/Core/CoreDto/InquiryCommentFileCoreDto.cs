namespace Modsen.Portal.Inquiries.Requests.Comments.Core.CoreDto;

internal class InquiryCommentFileCoreDto
{
    public int Id { get; set; }
    public required string FileId { get; set; }
    public string? FileName { get; set; }
    public long FileSize { get; set; }
}
