using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Controllers.Contracts.Output;

internal class InquiryTemplateOutput
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public InquiryType Type { get; init; }
    public TrackingUnit TrackingUnitType { get; set; }
    public ActivityType ActivityType { get; set; }
    public AccrualType? AccrualType { get; set; }
    public TransferType? TransferType { get; set; }
    public string? Comment { get; set; }
    public bool IsProhibitOnProbationPeriod { get; set; }
    public bool IsRequireAttachments { get; init; }
    public bool IsIncludeHolidays { get; init; }
    public bool IsIncludeWeekends { get; init; }
    public bool IsSubstitutesToApproveAllowed { get; init; }
    public bool HasAnyTransactions { get; set; }
    public int AssignedUsersCount { get; set; }
    public IReadOnlyCollection<InquiryTemplateAcceptPolicyOutput> AcceptPolicies { get; init; } = [];
    public IReadOnlyCollection<InquiryAgreementTemplateDto> Agreements { get; init; } = [];

    public static InquiryTemplateOutput Of(InquiryTemplateDto dto)
    {
        return new InquiryTemplateOutput
        {
            Id = dto.Id,
            Name = dto.Name,
            Type = dto.Type,
            IsRequireAttachments = dto.IsRequireAttachments,
            IsIncludeHolidays = dto.IsIncludeHolidays,
            IsIncludeWeekends = dto.IsIncludeWeekends,
            Agreements = dto.Agreements,
            TrackingUnitType = dto.TrackingUnit,
            ActivityType = dto.ActivityType,
            AccrualType = dto.AccrualType,
            TransferType = dto.TransferType,
            Comment = dto.Comment,
            IsProhibitOnProbationPeriod = dto.ProhibitOnProbationPeriod,
            IsSubstitutesToApproveAllowed = dto.IsSubstitutesToApproveAllowed,
            HasAnyTransactions = dto.HasAnyTransactions,
            AssignedUsersCount = dto.AssignedUsersCount,
            AcceptPolicies = dto.AcceptPolicies.ToList().ConvertAll(InquiryTemplateAcceptPolicyOutput.Of),
        };
    }
}
