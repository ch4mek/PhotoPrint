using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;

internal record CvUserCoreDto
{
    public int UserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public required string Email { get; set; }
    public string? DepartmentId { get; set; }
    public int? ResourceGroupId { get; set; }
    public string? Position { get; set; }
    public string? Location { get; set; }
    public bool IsActive { get; set; }

    public static CvUserCoreDto Of(CvUserEntity entity)
    {
        return new CvUserCoreDto
        {
            UserId = entity.UserId,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            FirstNameRu = entity.FirstNameRu,
            LastNameRu = entity.LastNameRu,
            Email = entity.Email,
            DepartmentId = entity.DepartmentId,
            ResourceGroupId = entity.ResourceGroupId,
            Position = entity.Position,
            Location = entity.Location,
            IsActive = entity.IsActive,
        };
    }
}
