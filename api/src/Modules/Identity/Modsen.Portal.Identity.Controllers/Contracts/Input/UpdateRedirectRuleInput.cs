namespace Modsen.Portal.Identity.Controllers.Contracts.Input;

public class UpdateRedirectRuleInput
{
    public required Guid Id { get; set; }
    public required string RedirectTo { get; set; }
}
