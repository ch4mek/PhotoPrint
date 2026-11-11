using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.CoreDto;

namespace Modsen.Portal.Identity.Requests.PermissionAssignments.Requests.RequestDto;

public class PermissionAssignmentDto
{
    public required int Id { get; set; }

    public string? ResourceId { get; set; }
           
    public string? ResourceName { get; set; }

    public required PermissionResourceType ResourceType { get; set; }

    public required int UserId { get; set; }

    public required PermissionType PermissionType { get; set; }

    public static PermissionAssignmentDto? Of(PermissionAssignmentCoreDto? source)
    {
        return source == null
            ? null
            : new PermissionAssignmentDto
            {
                Id = source.Id,
                ResourceId = source.ResourceId,
                ResourceName = source.ResourceName,
                ResourceType = source.ResourceType,
                UserId = source.UserId,
                PermissionType = source.PermissionType
            };
    }
}
