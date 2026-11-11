namespace Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;

internal class UserInquiryFileCoreDto
{
    public int Id { get; set; }
    public required string FileId { get; set; }
    public string? FileName { get; set; }
    public long FileSize { get; set; }
}
