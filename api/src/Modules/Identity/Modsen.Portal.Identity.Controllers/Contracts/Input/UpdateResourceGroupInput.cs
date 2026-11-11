namespace Modsen.Portal.Identity.Controllers.Contracts.Input;

public record UpdateResourceGroupInput
{
    public int? GroupManagerId { get; set; }
    public required string DepartmentId { get; set; }
    public required string Name { get; set; }
};
