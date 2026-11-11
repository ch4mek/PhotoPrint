namespace Modsen.Portal.Tools.PeopleForceDataComposer.API.Contract;

public class EmployeeModel
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public string? Email { get; set; }
    public string? PersonalEmail { get; set; }
    public string? MobileNumber { get; set; }
    public string? WorkPhoneNumber { get; set; }
    public string? DepartmentId { get; set; }
    public int? DepartmentManagerId { get; set; }
    public int? ReportingToId { get; set; }
    public string? ReportingToEmail { get; set; }
    public string? Position { get; set; }
    public DateOnly? HireDate { get; set; }
    public DateOnly? DateOfBirth { get; set; }
}
