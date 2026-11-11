namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Input;

public class UpdateClientInput
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? LegalEntityName { get; init; }
    public string? Color { get; set; }
    public string? Comments { get; set; }
    public int RegisteredLocation { get; set; }
    public int? SalesManagerId { get; set; }
}
