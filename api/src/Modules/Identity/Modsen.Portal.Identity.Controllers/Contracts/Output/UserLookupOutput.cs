using Modsen.Portal.Identity.Requests.Users.Requests.RequestDto;

namespace Modsen.Portal.Identity.Controllers.Contracts.Output;

public class UserLookupOutput
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public string? PositionDescription { get; set; }
    public string? DepartmentId { get; set; }
    public string? ResourceGroupName { get; set; }
    public string? Color { get; set; }
    public string? AvatarFileId { get; set; }
    public bool IsActive { get; set; }
    public int? ResourceGroupId { get; set; }

    public static UserLookupOutput Of(UserLookupDto source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return new UserLookupOutput
        {
            Id = source.Id,
            FirstName = source.FirstName,
            LastName = source.LastName,
            FirstNameRu = source.FirstNameRu,
            LastNameRu = source.LastNameRu,
            PositionDescription = source.PositionDescription,
            DepartmentId = source.DepartmentId,
            ResourceGroupName = source.ResourceGroupName,
            Color = source.Color,
            AvatarFileId = source.AvatarFileId,
            IsActive = source.IsActive,
            ResourceGroupId = source.ResourceGroupId,
        };
    }
}
