using System.Collections.ObjectModel;
using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.Identity.Controllers.Contracts.Input;

public class UpdateUserPermissionsInput
{
    public required int UserId { get; set; }
    public required PermissionType PermissionType { get; set; }
    public required IReadOnlyDictionary<string, string> Resources { get; set; } = ReadOnlyDictionary<string, string>.Empty;
    public required PermissionResourceType ResourceType { get; set; }
}
