namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

internal sealed class ContractDatesDto
{
    public DateOnly ContractFrom { get; set; }

    public DateOnly? ContractTo { get; set; }

    public DateOnly? FireDate { get; set; }
}
