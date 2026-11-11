using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.Inquiries.Requests.Assigns.Requests.RequestDtos;

internal class UserAssignRequestDto
{
    public decimal TrackingUnitCount { get; set; }
    public InquiryType Type { get; set; }
    public required string Name { get; set; }
    public TrackingUnit TrackingUnitType { get; set; }
    public bool IsAgreement { get; set; }
    public int InquiryTemplateId { get; set; }
    public int? AgreementTemplateId { get; set; }

    public DateOnly DateFrom { get; set; }
}
