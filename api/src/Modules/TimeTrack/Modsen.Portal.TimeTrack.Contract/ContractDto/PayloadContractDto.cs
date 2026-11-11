namespace Modsen.Portal.TimeTrack.Contract.ContractDto;

public class PayloadContractDto
{
    public int Year { get; set; }
    public IReadOnlyCollection<PayloadUserContractDto> Users { get; set; } = [];
    public IReadOnlyCollection<PayloadProjectContractDto> Projects { get; set; } = [];
    public IReadOnlyCollection<PayloadClientContractDto> Clients { get; set; } = [];
}
