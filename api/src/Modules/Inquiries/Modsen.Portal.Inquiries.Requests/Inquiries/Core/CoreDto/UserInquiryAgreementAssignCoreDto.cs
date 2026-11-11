using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;

internal class UserInquiryAgreementAssignCoreDto
{
    public int Id { get; set; }
    public int InquiryUserId { get; set; }
    public int? AgreementTemplateId { get; set; }
    public int InquiryTemplateId { get; set; }
    public string? InquiryTemplateName { get; set; }
    public bool IsAgreement { get; set; }
    public UserInquiryStatus Status { get; set; }
    public int StatusOrderWeight { get; set; }
    public string? Notes { get; set; }
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
    public decimal? TotalHours { get; set; }
    public decimal? TotalDays { get; set; }
    public decimal? TotalAccountantDays { get; set; }
    public decimal? CompensationAmount { get; set; }

    public IReadOnlyCollection<UserInquiryDateCoreDto> UserInquiryDates { get; set; } = [];
    public IReadOnlyCollection<UserInquiryAcceptCoreDto> UserInquiryAccepts { get; set; } = [];

    public static UserInquiryAgreementAssignCoreDto Of(UserInquiryCoreDto coreDto)
    {
        return new UserInquiryAgreementAssignCoreDto
        {
            Id = coreDto.Id,
            InquiryUserId = coreDto.InquiryUserId,
            AgreementTemplateId = null,
            InquiryTemplateId = coreDto.InquiryTemplateId,
            InquiryTemplateName = coreDto.InquiryTemplateName,
            IsAgreement = false,
            Status = coreDto.Status,
            StatusOrderWeight = coreDto.StatusOrderWeight,
            Notes = coreDto.Notes,
            DateFrom = coreDto.DateFrom,
            DateTo = coreDto.DateTo,
            TotalHours = coreDto.TotalHours,
            TotalDays = coreDto.TotalDays,
            TotalAccountantDays = coreDto.TotalAccountantDays,
            CompensationAmount = coreDto.CompensationAmount,
            UserInquiryDates = coreDto.UserInquiryDates,
            UserInquiryAccepts = coreDto.UserInquiryAccepts
        };
    }
}
