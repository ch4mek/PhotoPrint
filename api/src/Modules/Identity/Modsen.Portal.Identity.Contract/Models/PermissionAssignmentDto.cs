using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.Identity.Contract.Models;

public class PermissionAssignmentDto
{
    public required int Id { get; set; }

    public string? ResourceId { get; set; }

    public required PermissionResourceType ResourceType { get; set; }

    public required int UserId { get; set; }

    public required PermissionType PermissionType { get; set; }
}
