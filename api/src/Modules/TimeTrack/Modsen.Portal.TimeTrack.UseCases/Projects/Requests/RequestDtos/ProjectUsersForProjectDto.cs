namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

public class ProjectUsersForProjectDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? DepartmentId { get; set; }
}
