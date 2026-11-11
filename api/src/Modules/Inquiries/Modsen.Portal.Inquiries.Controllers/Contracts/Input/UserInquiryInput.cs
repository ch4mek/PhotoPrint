using Modsen.Portal.Inquiries.Requests.Inquiries.Request.RequestDto;

namespace Modsen.Portal.Inquiries.Controllers.Contracts.Input;

internal sealed record UserInquiryInput
{
    public int InquiryTemplateId { get; set; }
    public int? AgreementTemplateId { get; set; }
    public string? Notes { get; set; }
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
    public int? Hours { get; set; }
    public IReadOnlyCollection<CreateInquiryFileDto> Files { get; set; } = [];
}

