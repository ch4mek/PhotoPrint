namespace Modsen.Portal.Identity.Contract.Models;

public class DepartmentDto
{
    public required string Id { get; set; }
    public required string ShortName { get; set; }
    public required string FullName { get; set; }
    public int DepartmentOrderWeight { get; init; }
    public required string DepartmentReportingToRoleName { get; set; }
    public int DepartmentReportingToRoleId { get; set; }
}
