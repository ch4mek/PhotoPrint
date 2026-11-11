namespace Modsen.Portal.Inquiries.Requests.Inquiries.Request.RequestDto;

public record CreateInquiryFileDto(
    string FileId,
    string? FileName,
    long FileSize);

