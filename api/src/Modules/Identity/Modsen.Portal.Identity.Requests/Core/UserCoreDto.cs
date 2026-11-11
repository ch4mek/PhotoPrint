using System.Linq.Expressions;
using Modsen.Portal.Core;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Identity.Requests.Core;

public class UserCoreDto
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
    public string? Color { get; set; }
    public string? AvatarColor { get; set; }
    public string? AvatarFileId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public string? FullNameRu { get; set; }
    public string? CombinedFullName { get; set; }
    public string Email { get; set; } = null!;
    public string? TimeZone { get; set; }
    public string? DepartmentId { get; set; }
    public string? DepartmentFullName { get; set; }
    public int? ResourceGroupId { get; set; }
    public string? ResourceGroupName { get; set; }
    public LocationType? Location { get; set; }
    public int? RegisteredLocationId { get; set; } = null!;
    public string Position { get; set; } = null!;
    public string PositionPublicName { get; set; } = null!;
    public int PositionOrderWeight { get; set; }

    internal static readonly Expression<Func<User, UserCoreDto>> MapUser =
        x => new UserCoreDto
        {
            Id = x.Id,
            IsActive = x.IsActive,
            Color = x.Color,
            AvatarColor = x.AvatarColor,
            AvatarFileId = x.AvatarFileId,
            FirstName = x.FirstName,
            LastName = x.LastName,
            FullName = x.LastName + " " + x.FirstName,
            FirstNameRu = x.FirstNameRu,
            LastNameRu = x.LastNameRu,
            FullNameRu = x.LastNameRu == null || x.FirstNameRu == null ? null : x.LastNameRu + " " + x.FirstNameRu,
            CombinedFullName = x.LastNameRu == null && x.FirstNameRu == null
                ? x.LastName + " " + x.FirstName
                : x.LastNameRu != null && x.FirstNameRu != null
                    ? x.LastNameRu + " " + x.FirstNameRu + " (" + x.LastName + " " + x.FirstName + ")"
                    : (x.LastNameRu ?? x.FirstNameRu) + " (" + x.LastName + " " + x.FirstName + ")",
            Email = x.Email!,
            TimeZone = x.TimeZone,
            DepartmentId = x.DepartmentId,
            DepartmentFullName = x.Department != null ? x.Department.FullName : null,
            ResourceGroupId = x.ResourceGroupId,
            ResourceGroupName = x.ResourceGroup != null && x.ResourceGroup.IsActive ? x.ResourceGroup.Name : null,
            Location = x.Location,
            RegisteredLocationId = x.RegisteredLocationId,
            Position = x.UserRoles.First().Role!.Name!,
            PositionPublicName = x.UserRoles.First().Role!.PublicName,
            PositionOrderWeight = x.UserRoles.First().Role!.OrderWeight
        };
}



