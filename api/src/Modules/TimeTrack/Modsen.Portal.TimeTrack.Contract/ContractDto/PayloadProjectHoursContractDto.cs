namespace Modsen.Portal.TimeTrack.Contract.ContractDto;

public class PayloadProjectHoursContractDto
{
    public DateOnly UserOnProjectFrom { get; set; }
    public DateOnly? UserOnProjectTo { get; set; }
    public int ProjectId { get; set; }
    public int TotalFactTrackedInMinutes { get; set; }
    public int TotalBillableTrackedInMinutes { get; set; }
    public bool IsActive { get; set; }
}
