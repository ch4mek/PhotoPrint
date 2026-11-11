namespace Modsen.Portal.Identity.Controllers.Contracts.Input;

public class CreateResourceGroupInput
{
    public int? GroupManagerId { get; set; }
    public required string GroupName { get; set; }
}
