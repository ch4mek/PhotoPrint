using Modsen.Portal.Core;

namespace Modsen.Portal.Identity.Requests.Dto;

public class UserDto
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public string? FullNameRu { get; set; }
    public string? PatronymicRu { get; set; }
    public string? Color { get; set; }
    public int? CountryId { get; set; }
    public string? AvatarColor { get; set; }
    public string? AvatarFileId { get; set; }
    public required string Email { get; set; }
    public string? TimeZone { get; set; }
    public string? Position => RoleNames.FirstOrDefault();
    public string? PositionPublicName { get; set; }
    public string? DepartmentId { get; set; }
    public int? ResourceGroupId { get; set; }
    public string? ResourceGroupName { get; set; }
    public LocationType? Location { get; set; }
    public int RegisteredLocationId { get; set; }
    public string? CurrentLocation { get; set; }
    public UserSignatureDto Signature { get; set; } = new();
    public bool IsActive { get; set; }
    public DateOnly? HireDate { get; set; }
    public IReadOnlyCollection<string> RoleNames { get; set; } = new List<string>();

    public int? PositionOrderWeight { get; set; }

    public int? CreatedById { get; set; }
}
