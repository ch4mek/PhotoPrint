namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

internal class ProjectModelDto
{
    public required int Id { get; set; }
    public required bool IsPublic { get; set; }
    public required bool IsActive { get; set; }
}
