namespace Modsen.Portal.Identity.Controllers.Contracts.Output;

public class DepartmentOutput
{
    public DepartmentOutput(string departmentName, IReadOnlyCollection<DepartmentGroupOutput>? groups = null, string? headOfDepartmentId = null)
    {
        DepartmentName = departmentName;
        HeadOfDepartmentId = headOfDepartmentId;
        Groups = groups ?? [];
    }

    public string DepartmentName { get; set; }
    public string? HeadOfDepartmentId { get; set; }

    public IReadOnlyCollection<DepartmentGroupOutput> Groups { get; init; }
}
