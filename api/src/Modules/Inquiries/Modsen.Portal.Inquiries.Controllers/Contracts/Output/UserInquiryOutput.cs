using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Controllers.Contracts.Output;

internal class UserInquiryOutput
{
    public int Id { get; set; }
    public int InquiryUserId { get; set; }

    public int? AgreementTemplateId { get; set; }
    public int InquiryTemplateId { get; set; }
    public string? InquiryTemplateName { get; set; }
    public string? InquiryType { get; set; }
    public bool IsAgreement { get; set; }

    public string? Status { get; set; }
    public int StatusOrderWeight { get; set; }
    public string? Notes { get; set; }
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
    public decimal? TotalHours { get; set; }
    public decimal? TotalDays { get; set; }
    public decimal? TotalAccountantDays { get; set; }
    public decimal? CompensationAmount { get; set; }

    public bool? CanCurrentUserApprove { get; set; }
    public bool? CanCurrentUserReject { get; set; }

    public int CountryId { get; set; }

    public int? CreatedById { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public decimal? RemainingDays { get; set; }
    public decimal? BalanceDays { get; set; }
    public decimal? BalanceHours { get; set; }

    public int? UpdatedById { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public IReadOnlyCollection<UserInquiryDateOutput> UserInquiryDates { get; set; } = [];
    public IReadOnlyCollection<UserInquiryAcceptOutput> UserInquiryAccepts { get; set; } = [];
    public IReadOnlyCollection<UserInquiryFileCoreDto> UserInquiryFiles { get; set; } = [];

    public InquiriesUserOutput? CreatedByUser { get; set; }
    public InquiriesUserOutput? CreatedForUser { get; set; }
    public InquiriesUserOutput? ManagerOfUser { get; set; }

    public static UserInquiryOutput Of(UserInquiryCoreDto coreDto)
    {
        return new UserInquiryOutput
        {
            Id = coreDto.Id,
            InquiryUserId = coreDto.InquiryUserId,
            AgreementTemplateId = coreDto.AgreementTemplateId,
            InquiryTemplateId = coreDto.InquiryTemplateId,
            InquiryTemplateName = coreDto.InquiryTemplateName,
            InquiryType = coreDto.InquiryType,
            IsAgreement = coreDto.IsAgreement,
            Status = coreDto.Status.GetDescription(),
            StatusOrderWeight = coreDto.StatusOrderWeight,
            Notes = coreDto.Notes,
            DateFrom = coreDto.DateFrom,
            DateTo = coreDto.DateTo,
            TotalHours = coreDto.TotalHours,
            TotalDays = coreDto.TotalDays,
            TotalAccountantDays = coreDto.TotalAccountantDays,
            CompensationAmount = coreDto.CompensationAmount,
            RemainingDays = coreDto.HistoricalRemainingDays,
            BalanceDays = coreDto.HistoricalBalanceDays,
            BalanceHours = coreDto.HistoricalBalanceHours,
            CreatedById = coreDto.CreatedById,
            CreatedAtUtc = coreDto.CreatedAtUtc,
            UpdatedById = coreDto.UpdatedById,
            UpdatedAtUtc = coreDto.UpdatedAtUtc,
            UserInquiryDates = coreDto.UserInquiryDates
                .ConvertAll(UserInquiryDateOutput.Of),
            UserInquiryAccepts = coreDto.UserInquiryAccepts
                .ConvertAll(UserInquiryAcceptOutput.Of),
            CanCurrentUserApprove = coreDto.CanCurrentUserApprove,
            CanCurrentUserReject = coreDto.CanCurrentUserReject,
            UserInquiryFiles = coreDto.UserInquiryFiles,
            CountryId = coreDto.CountryId,

            CreatedByUser = InquiriesUserOutput.Of(coreDto.CreatedByUser),
            CreatedForUser = InquiriesUserOutput.Of(coreDto.CreatedForUser),
            ManagerOfUser = InquiriesUserOutput.Of(coreDto.ManagerOfUser),
        };
    }
}
