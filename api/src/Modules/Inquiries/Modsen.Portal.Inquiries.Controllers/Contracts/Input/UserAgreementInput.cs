using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.Inquiries.Controllers.Contracts.Input;

internal class UserAgreementInput
{
    public AgreementType Type { get; init; }
    public AccrualType AccrualType { get; init; }
    public DateOnly DateFrom { get; init; }
    public DateOnly DateTo { get; init; }
    public DateOnly? FreezeUntilDate { get; init; }
    public int DaysPerContractYear { get; init; }
    public bool IsStaked { get; set; }
    public bool AllowNegativeBalanceInDays { get; set; }
    public int? NegativeBalanceInDaysMax { get; set; }
}
