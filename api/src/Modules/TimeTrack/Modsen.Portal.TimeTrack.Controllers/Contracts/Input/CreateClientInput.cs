namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Input;

public class CreateClientInput
{
    public required string Name { get; set; }
    public string? LegalEntityName { get; set; }
    public string? Comments { get; set; }
    public string? Color { get; set; }
    public int RegisteredLocation { get; set; }
    public int? SalesManagerId { get; set; }
}
