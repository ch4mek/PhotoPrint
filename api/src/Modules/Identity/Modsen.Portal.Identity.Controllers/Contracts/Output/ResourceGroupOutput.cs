namespace Modsen.Portal.Identity.Controllers.Contracts.Output;

public class ResourceGroupOutput
{
    public int Id { get; set; }
    public int? GroupManagerId { get; set; }
    public string? ManagerFirstName { get; set; }
    public string? ManagerLastName { get; set; }
    public required string Name { get; set; }
    public required string DepartmentId { get; set; }
};
