namespace Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;

internal class UserWithWorkingDaysDto
{
    public int EmployeeId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public string? Email { get; set; }
    public string? DepartmentId { get; set; }
    public string? Position { get; set; }
    public IReadOnlyCollection<DayOfMonthDto> DaysOfMonth { get; set; } = [];
}
