using Modsen.Portal.Core;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Dto;

namespace Modsen.Portal.Identity.Controllers.Contracts.Output;

public class UserOutput
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string FullName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public string? FullNameRu { get; set; }
    public string? PatronymicRu { get; set; }
    public string? Color { get; set; }
    public int? CountryId { get; set; }
    public int? RegisteredLocationId { get; set; }
    public string? CurrentLocation { get; init; }
    public string? AvatarFileId { get; set; }
    public required string Email { get; set; }
    public string? TimeZone { get; set; }
    public string? Position { get; set; }
    public string? RightPosition { get; set; }
    public string? DepartmentId { get; set; }
    public int? ResourceGroupId { get; set; }
    public string? ResourceGroupName { get; set; }
    public LocationType? Location { get; set; }
    public bool IsActive { get; set; }

    public static UserOutput Of(UserDto source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return new UserOutput
        {
            Id = source.Id,
            FirstName = source.FirstName,
            LastName = source.LastName ?? string.Empty,
            FullName = source.FullName ?? string.Empty,
            FirstNameRu = source.FirstNameRu,
            LastNameRu = source.LastNameRu,
            FullNameRu = source.FullNameRu,
            PatronymicRu = source.PatronymicRu,
            Color = source.Color,
            CountryId = source.CountryId,
            RegisteredLocationId = source.RegisteredLocationId,
            AvatarFileId = source.AvatarFileId,
            Email = source.Email,
            TimeZone = source.TimeZone,
            Position = source.Position,
            RightPosition = source.PositionPublicName,
            DepartmentId = source.DepartmentId,
            ResourceGroupId = source.ResourceGroupId,
            ResourceGroupName = source.ResourceGroupName,
            Location = source.Location,
            IsActive = source.IsActive,
            CurrentLocation = source.CurrentLocation,
        };
    }

    public static UserOutput Of(UserCoreDto source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return new UserOutput
        {
            Id = source.Id,
            FirstName = source.FirstName,
            LastName = source.LastName,
            FullName = source.FullName,
            FirstNameRu = source.FirstNameRu,
            LastNameRu = source.LastNameRu,
            FullNameRu = source.FullNameRu,
            Color = source.Color,
            RegisteredLocationId = source.RegisteredLocationId,
            AvatarFileId = source.AvatarFileId,
            Email = source.Email,
            TimeZone = source.TimeZone,
            Position = source.Position,
            RightPosition = source.PositionPublicName,
            DepartmentId = source.DepartmentId,
            ResourceGroupId = source.ResourceGroupId,
            ResourceGroupName = source.ResourceGroupName,
            Location = source.Location,
            IsActive = source.IsActive,
        };
    }
}
