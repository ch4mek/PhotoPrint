namespace Modsen.Portal.TimeTrack.Contract.ContractDto;

public class PayloadClientContractDto
{
    public int ClientId { get; set; }
    public required string Name { get; set; }
    public string? Color { get; set; }
}
