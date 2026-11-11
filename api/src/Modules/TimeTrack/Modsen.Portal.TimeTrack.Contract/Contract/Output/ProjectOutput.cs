namespace Modsen.Portal.TimeTrack.Contract.Contract.Output;

public class ProjectOutput
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? Color { get; set; }
    public int ClientId { get; set; }
    public bool IsBillable { get; set; }
    public bool IsActive { get; set; }
    public bool IsPublic { get; set; }
    public IReadOnlyCollection<ProjectUserOutput> ProjectUsers { get; set; } = [];
}
