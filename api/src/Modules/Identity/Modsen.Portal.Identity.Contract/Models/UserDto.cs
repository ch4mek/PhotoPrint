using Modsen.Portal.Core;

namespace Modsen.Portal.Identity.Contract.Models;

public class UserDto
{
    public required int UserId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? FirstNameRu { get; init; }
    public string? LastNameRu { get; init; }
    public string? PatronymicRu { get; init; }
    public required string? Email { get; init; }
    public string? DepartmentId { get; init; }
    public int? ResourceGroupId { get; init; }
    public string? Color { get; init; }
    public int? CountryId { get; init; }
    public string? AvatarFileId { get; init; }
    public IReadOnlyCollection<string> RoleNames { get; set; } = [];
    public string? Position => RoleNames.FirstOrDefault();
    public LocationType? Location { get; set; }
    public int PositionOrderWeight { get; init; }
    public int RegisteredLocationId { get; init; }
}
