namespace Modsen.Portal.TeamBoard.Requests.Contracts.Requests.RequestDtos;

internal class UserContractDatesDto
{
    public int UserId { get; set; }
    public DateOnly? HireDate { get; set; }
    public DateOnly? ContractStartDate { get; set; }
    public DateOnly? ContractEndDate { get; set; }
    public DateOnly? FireDate { get; set; }
}
