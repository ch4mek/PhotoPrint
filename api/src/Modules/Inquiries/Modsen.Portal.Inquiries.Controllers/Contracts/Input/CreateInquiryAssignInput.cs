namespace Modsen.Portal.Inquiries.Controllers.Contracts.Input;

internal sealed record CreateInquiryAssignInput(
    int[] AssigneUserIds,
    int[] UnassigneUserIds,
    int InquiryTemplateId,
    int? AgreementId,
    DateOnly? EffectiveFrom);
