namespace Modsen.Portal.Identity.Controllers.Contracts.Input;

public class UpdateUserSignatureInput
{
    public required string FullName { get; set; }
    public required string Position { get; set; }
    public string? Phone { get; set; }
}
