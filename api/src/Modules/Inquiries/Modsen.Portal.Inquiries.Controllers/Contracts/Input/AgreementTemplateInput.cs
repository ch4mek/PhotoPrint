namespace Modsen.Portal.Inquiries.Controllers.Contracts.Input;

internal sealed class AgreementTemplateInput
{
    public required string Name { get; set; }
    public int DaysPerContractYear { get; set; }
    public bool IsDefault { get; set; }
}
