using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Controllers.Contracts.Input;

internal class InquiryTemplateInput
{
    public required string Name { get; init; }
    public InquiryType Type { get; init; }
    public TrackingUnit TrackingUnitType { get; init; }
    public AccrualType? AccrualType { get; set; }
    public TransferType? TransferType { get; set; }
    public ActivityType ActivityType { get; init; }
    public string? Comment { get; init; }

    public bool IsProhibitOnProbationPeriod { get; init; }
    public bool IsRequireAttachments { get; init; }
    public bool IsIncludeHolidays { get; init; }
    public bool IsIncludeWeekends { get; init; }
    public bool IsSubstitutesToApproveAllowed { get; init; }
    public IReadOnlyCollection<UpdateInquiryTemplateAcceptPolicyDto> AcceptPolicies { get; init; } = [];
}
