namespace Modsen.Portal.TeamBoard.Contract.Contract.Output;

public class UserContractDatesOutput
{
    public required int UserId { get; set; }
    public required DateOnly? HireDate { get; set; }
    public required DateOnly? ContractStartDate { get; set; }
    public required DateOnly? ContractEndDate { get; set; }
    public required DateOnly? FireDate { get; set; }
}
