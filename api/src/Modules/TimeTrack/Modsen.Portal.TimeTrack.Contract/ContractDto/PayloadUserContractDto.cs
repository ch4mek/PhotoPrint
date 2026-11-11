namespace Modsen.Portal.TimeTrack.Contract.ContractDto;

public class PayloadUserContractDto
{
    public int UserId { get; set; }
    public IReadOnlyCollection<PayloadUserMonthHoursContractDto> HourDetails { get; set; } = [];
}
