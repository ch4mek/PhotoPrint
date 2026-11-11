namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

internal class ProjectWithManagerDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Color { get; set; }
    public required int ClientId { get; set; }
    public required bool IsBillable { get; set; }
    public required bool IsActive { get; set; }
    public required bool IsPublic { get; set; }
    public bool? IsSelfCreatedProject { get; set; }
    public int? ManagerId { get; set; }
    public string? ManagerFullName { get; set; }
}
