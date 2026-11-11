using Modsen.Portal.Core;

namespace Modsen.Portal.Identity.Controllers.Contracts.Input;

public class UpdateCurrentUserInput
{
    public required string TimeZone { get; set; }
    public string? AvatarColor { get; set; }
    public string? AvatarFileId { get; set; }
    public string? DepartmentId { get; set; }
    public required LocationType Location { get; set; }
}
