using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.CoreDto;

namespace Modsen.Portal.Identity.Controllers.Contracts.Output;

public class ProjectPermissionAssignmentOutput
{
    public int AssignmentId { get; set; }

    public int UserId { get; set; }

    public string? UserFullName { get; set; }

    public string? ProjectId { get; set; }

    public string? ProjectName { get; set; }

    public string? UserColor { get; set; }

    public static ProjectPermissionAssignmentOutput Of(ProjectPermissionAssignmentCoreDto source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return new ProjectPermissionAssignmentOutput
        {
            AssignmentId = source.Id,
            ProjectId = source.ProjectId,
            ProjectName = source.ProjectName,
            UserColor = source.UserColor,
            UserFullName = source.UserFullName,
            UserId = source.UserId
        };
    }
}
