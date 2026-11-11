using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Output;

public class ProjectUserInfoOutput
{
    public required int UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? DepartmentId { get; set; }

    public static ProjectUserInfoOutput? Of(ProjectUserInfoDto? source)
    {
        if (source == null)
        {
            return null;
        }

        return new ProjectUserInfoOutput
        {
            UserId = source.Id,
            FirstName = source.FirstName,
            LastName = source.LastName,
            DepartmentId = source.DepartmentId
        };
    }
}
