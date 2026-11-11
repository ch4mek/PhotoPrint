using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;

internal class AbsenceHistoryQueryUnionCoreDto
{
    public decimal AmountOfDays { get; set; }
    public DateTime TransactionDate { get; set; }
    public UserAgreementTransactionType? TransactionType { get; set; }
    public string? InquiryTemplateName { get; set; }
    public int InquiryTemplateId { get; set; }
    public int? InquiryId { get; set; }
    public int? UserAgreementId { get; set; }
    public decimal? Used { get; set; }
    public decimal? Accrued { get; set; }
    public decimal? Remainder { get; set; }
    public InquiryType InquiryType { get; set; }
    public int InquiryTypeOrderWeight { get; set; }
    public TrackingUnit TrackingUnitType { get; set; }
    public string? Description { get; set; }
    public bool IsCreatedManually { get; set; }
    public IReadOnlyCollection<InquiryFileCoreDto> Documents { get; set; } = [];
}
