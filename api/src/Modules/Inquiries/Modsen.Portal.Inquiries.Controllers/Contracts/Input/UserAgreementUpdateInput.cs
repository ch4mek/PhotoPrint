namespace Modsen.Portal.Inquiries.Controllers.Contracts.Input;

internal class UserAgreementUpdateInput
{
    public DateOnly DateTo { get; init; }
    public DateOnly? FreezeUntilDate { get; init; }
    public int DaysPerContractYear { get; init; }
}
