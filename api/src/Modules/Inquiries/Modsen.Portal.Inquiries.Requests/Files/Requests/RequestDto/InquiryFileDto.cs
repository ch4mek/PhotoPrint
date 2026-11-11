namespace Modsen.Portal.Inquiries.Requests.Files.Requests.RequestDto;

internal class InquiryFileDto
{
    public int Id { get; set; }
    public required string FileId { get; set; }

    public int? CreatedById { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public int? UpdatedById { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
