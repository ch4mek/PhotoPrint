namespace Modsen.Portal.TimeTrack.Contract.ContractDto;

public class PayloadProjectContractDto
{
    public int ProjectId { get; set; }
    public int ClientId { get; set; }
    public required string Name { get; set; }
}
