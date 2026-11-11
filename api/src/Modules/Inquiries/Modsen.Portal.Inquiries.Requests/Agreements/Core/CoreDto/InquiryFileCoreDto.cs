namespace Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;

public record InquiryFileCoreDto(
    string FileId,
    string? FileName,
    long FileSize);
