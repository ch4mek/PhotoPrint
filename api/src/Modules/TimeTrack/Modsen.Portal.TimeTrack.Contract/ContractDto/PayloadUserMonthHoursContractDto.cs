namespace Modsen.Portal.TimeTrack.Contract.ContractDto;

public class PayloadUserMonthHoursContractDto
{
    public int MonthOfYear { get; set; }
    public int TotalFactTrackedInMinutes { get; set; }
    public int TotalBillableTrackedInMinutes { get; set; }
    public IReadOnlyCollection<PayloadProjectHoursContractDto> Projects { get; set; } = [];
}
