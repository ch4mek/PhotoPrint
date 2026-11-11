using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Requests.RequestDto;

namespace Modsen.Portal.Identity.Controllers.Contracts.Output;

public class PermissionAssignmentOutput
{
    public required int AssignmentId { get; set; }
    public string? ResourceId { get; set; }
    public string? ResourceName { get; set; }
    public required PermissionResourceType ResourceType { get; set; }
    public required int UserId { get; set; }
    public required PermissionType PermissionType { get; set; }

    public static PermissionAssignmentOutput Of(PermissionAssignmentDto assignmentDto)
    {
        ArgumentNullException.ThrowIfNull(assignmentDto);
        return new PermissionAssignmentOutput
        {
            AssignmentId = assignmentDto.Id,
            ResourceId = assignmentDto.ResourceId,
            ResourceName = assignmentDto.ResourceName,
            ResourceType = assignmentDto.ResourceType,
            PermissionType = assignmentDto.PermissionType,
            UserId = assignmentDto.UserId
        };
    }
}
