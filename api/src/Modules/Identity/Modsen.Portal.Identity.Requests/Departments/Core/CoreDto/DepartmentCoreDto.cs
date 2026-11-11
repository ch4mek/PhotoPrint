namespace Modsen.Portal.Identity.Requests.Departments.Core.CoreDto;

internal class DepartmentCoreDto
{
    public required string DepartmentName { get; set; }
    public string? HeadOfDepartmentId { get; set; }
    public IReadOnlyCollection<DepartmentGroupCoreDto> Groups { get; set; } = [];
}
