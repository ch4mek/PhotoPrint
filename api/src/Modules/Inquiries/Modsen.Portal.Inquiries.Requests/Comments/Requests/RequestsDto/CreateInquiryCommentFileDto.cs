namespace Modsen.Portal.Inquiries.Requests.Comments.Requests.RequestsDto;

public record CreateInquiryCommentFileDto(
    string FileId,
    string? FileName,
    long FileSize);
