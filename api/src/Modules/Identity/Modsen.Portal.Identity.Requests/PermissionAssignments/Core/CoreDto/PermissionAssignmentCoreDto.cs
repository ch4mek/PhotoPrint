using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Identity.Requests.PermissionAssignments.Core.CoreDto;

public class PermissionAssignmentCoreDto
{
    public required int Id { get; set; }

    public string? ResourceId { get; set; }

    public string? ResourceName { get; set; }

    public required PermissionResourceType ResourceType { get; set; }

    public required int UserId { get; set; }

    public required PermissionType PermissionType { get; set; }

    internal static PermissionAssignmentCoreDto Of(PermissionAssignment source)
    {
        return new PermissionAssignmentCoreDto
        {
            Id = source.Id,
            ResourceId = source.ResourceId,
            ResourceName = source.ResourceName,
            ResourceType = Enum.Parse<PermissionResourceType>(source.ResourceType ?? string.Empty),
            PermissionType = Enum.Parse<PermissionType>(source.Permission?.Name ?? string.Empty),
            UserId = source.UserId
        };
    }
}
