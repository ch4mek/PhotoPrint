using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Controllers.Contracts.Output;

internal class UserInquiryAgreementOutput
{
    public int Id { get; set; }
    public int InquiryUserId { get; set; }
    public int? AgreementTemplateId { get; set; }
    public int InquiryTemplateId { get; set; }
    public string? InquiryTemplateName { get; set; }
    public bool IsAgreement { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
    public decimal? TotalHours { get; set; }
    public decimal? TotalDays { get; set; }
    public decimal? TotalAccountantDays { get; set; }
    public decimal? CompensationAmount { get; set; }

    public IReadOnlyCollection<UserInquiryDateOutput> UserInquiryDates { get; set; } = [];
    public IReadOnlyCollection<UserInquiryAcceptOutput> UserInquiryAccepts { get; set; } = [];

    public static UserInquiryAgreementOutput Of(UserInquiryAgreementAssignCoreDto coreDto)
    {
        return new UserInquiryAgreementOutput
        {
            Id = coreDto.Id,
            InquiryUserId = coreDto.InquiryUserId,
            AgreementTemplateId = coreDto.AgreementTemplateId,
            InquiryTemplateId = coreDto.InquiryTemplateId,
            InquiryTemplateName = coreDto.InquiryTemplateName,
            IsAgreement = coreDto.IsAgreement,
            Status = coreDto.Status.GetDescription(),
            Notes = coreDto.Notes,
            DateFrom = coreDto.DateFrom,
            DateTo = coreDto.DateTo,
            TotalHours = coreDto.TotalHours,
            TotalDays = coreDto.TotalDays,
            TotalAccountantDays = coreDto.TotalAccountantDays,
            CompensationAmount = coreDto.CompensationAmount,
            UserInquiryDates = coreDto.UserInquiryDates.ToList().ConvertAll(UserInquiryDateOutput.Of),
            UserInquiryAccepts = coreDto.UserInquiryAccepts.ToList().ConvertAll(UserInquiryAcceptOutput.Of),
        };
    }
}
