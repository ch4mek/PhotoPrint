namespace Modsen.Portal.Finance.Requests.Managements.Requests.RequestDtos;

internal class ManagementUserDto
{
    public int Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? FirstNameRu { get; init; }
    public string? LastNameRu { get; init; }
    public string? DepartmentId { get; init; }
    public bool IsActive { get; init; }

    public List<ManagementProjectDto> Projects { get; set; } = [];

    public decimal BillableTrackedHoursAmount { get; set; }
    public decimal HourlyRateAmount { get; set; }
    public decimal IncomeAmount { get; set; }
    public decimal IncomeBynAmount { get; set; }
    public decimal? Salary { get; set; }
    public int Kpi { get; set; }
    public string? KpiColor { get; set; }

    public bool CanUserInfoBeRetrievedByCurrentUser { get; set; }
}
