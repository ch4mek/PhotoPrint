using Modsen.Portal.TimeTrack.Domain;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

public class ProjectUserInfoDto
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? DepartmentId { get; set; }

    public static ProjectUserInfoDto Of(ProjectUser source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return new ProjectUserInfoDto
        {
            Id = source.Id,
            FirstName = source.FirstName,
            LastName = source.LastName,
            DepartmentId = source.DepartmentId,
        };
    }
}
