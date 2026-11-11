namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Input;

public class UpdateProjectInput
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? Color { get; set; }
    public int? ManagerId { get; set; }
    public required int ClientId { get; set; }
    public required bool IsPublic { get; set; }
    public required bool IsBillable { get; set; }
    public string? TechStack { get; set; }
    public string? Industry { get; set; }
    public string? Architecture { get; set; }
    public string? CloudServices { get; set; }
}
