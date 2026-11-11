namespace Modsen.Portal.Identity.Controllers.Contracts.Input;

public class UserClaimsInput
{
    public IReadOnlyCollection<string> ClaimTypes { get; set; } = [];
}
