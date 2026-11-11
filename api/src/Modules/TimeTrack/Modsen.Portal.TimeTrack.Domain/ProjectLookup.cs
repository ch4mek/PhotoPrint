namespace Modsen.Portal.TimeTrack.Domain;

public class ProjectLookup
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string ClientName { get; set; }
    public required bool IsBillable { get; set; }
    public string? Color { get; set; }
}
