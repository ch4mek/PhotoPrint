namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

public class UpdateProjectDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? Color { get; set; }
    public required int ClientId { get; set; }
    public int? ManagerId { get; set; }
    public required string ClientName { get; set; }
    public required bool IsPublic { get; set; }
    public required bool IsActive { get; set; }
    public required bool IsBillable { get; set; }
    public required bool IsCreatedByCurrentUser { get; set; }
    public required int TrackedTime { get; set; }
}
