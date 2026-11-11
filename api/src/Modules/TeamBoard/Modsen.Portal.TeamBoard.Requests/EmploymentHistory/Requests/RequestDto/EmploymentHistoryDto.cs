namespace Modsen.Portal.TeamBoard.Requests.EmploymentHistory.Requests.RequestDto;

public class EmploymentHistoryDto
{
    public DateOnly? ProfessionalActivityStartDate { get; set; }
    public DateOnly? FirstProjectStartDate { get; set; }
    public DateOnly? ContractEndDate { get; set; }
    public DateOnly? HireDate { get; set; }
    public DateOnly? FireDate { get; set; }
    public DateOnly? NdaStartDate { get; set; }
    public DateOnly? NdaEndDate { get; set; }
    public bool ShouldDisplayNda { get; set; }
}
