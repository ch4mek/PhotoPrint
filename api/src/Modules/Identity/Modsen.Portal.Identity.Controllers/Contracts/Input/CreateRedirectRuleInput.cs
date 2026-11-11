namespace Modsen.Portal.Identity.Controllers.Contracts.Input;

public class CreateRedirectRuleInput
{
    public string? RedirectKey { get; set; }
    public required string RedirectTo { get; set; }
}
