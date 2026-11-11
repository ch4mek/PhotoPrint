using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.CoreDto;

internal class InquiryTemplateDto
{
    public int Id { get; set; }
    public int OrderWeight { get; set; }
    public required string Name { get; init; }
    public InquiryType Type { get; init; }
    public TrackingUnit TrackingUnit { get; set; }
    public ActivityType ActivityType { get; set; }
    public TransferType? TransferType { get; set; }
    public AccrualType? AccrualType { get; set; }
    public string? Comment { get; set; }
    public bool ProhibitOnProbationPeriod { get; set; }
    public bool IsRequireAttachments { get; init; }
    public bool IsIncludeHolidays { get; init; }
    public bool IsIncludeWeekends { get; init; }
    public bool IsSubstitutesToApproveAllowed { get; init; }
    public bool HasAnyTransactions { get; set; }
    public int AssignedUsersCount { get; set; }
    public IReadOnlyCollection<InquiryTemplateAcceptPolicyDto> AcceptPolicies { get; init; } = [];
    public IReadOnlyCollection<InquiryAgreementTemplateDto> Agreements { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}
