namespace Modsen.Portal.Identity.Controllers.Contracts.Output;

public class UserPermissionResourcesOutput
{
    public int UserId { get; set; }
    public IReadOnlyCollection<ResourceOutput> Resources { get; set; } = [];
}
