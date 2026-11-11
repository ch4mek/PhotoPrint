using Modsen.Portal.Core;

namespace Modsen.Portal.Identity.Controllers.Contracts.Output;

public class CurrentUserOutput
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Color { get; set; }
    public string? AvatarColor { get; set; }
    public string? AvatarFileId { get; set; }
    public required string Email { get; set; }
    public string? TimeZone { get; set; }
    public string? Position { get; set; }
    public string? DepartmentId { get; set; }
    public LocationType? Location { get; set; }
    public int RegisteredLocationId { get; set; }
}
