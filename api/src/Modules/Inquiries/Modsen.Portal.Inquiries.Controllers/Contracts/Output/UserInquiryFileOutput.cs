using Modsen.Portal.Inquiries.Requests.Files.Requests.RequestDto;

namespace Modsen.Portal.Inquiries.Controllers.Contracts.Output;

internal class UserInquiryFileOutput
{
    public int Id { get; set; }
    public required string FileId { get; set; }

    public int? CreatedById { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public int? UpdatedById { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public static UserInquiryFileOutput Of(InquiryFileDto inquiryFileDto)
    {
        return new UserInquiryFileOutput
        {
            Id = inquiryFileDto.Id,
            FileId = inquiryFileDto.FileId,
            CreatedById = inquiryFileDto.CreatedById,
            CreatedAtUtc = inquiryFileDto.CreatedAtUtc,
            UpdatedById = inquiryFileDto.UpdatedById,
            UpdatedAtUtc = inquiryFileDto.UpdatedAtUtc
        };
    }
}
