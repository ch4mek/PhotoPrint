using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

internal record CreateOrUpdateUserCoreDto
{
    public int UserId { get; init; }
    public required string LastName { get; init; }
    public required string FirstName { get; init; }
    public string? FirstNameRu { get; init; }
    public string? LastNameRu { get; init; }
    public string? PatronymicRu { get; init; }
    public required string Email { get; init; }
    public string? DepartmentId { get; init; }
    public string? Color { get; init; }
    public int? CountryId { get; init; }
    public string? AvatarColor { get; init; }
    public string? AvatarFileId { get; init; }
    public string? Position { get; init; }
    public int PositionOrderWeight { get; init; }
    public string? Location { get; set; }
    public int? ResourceGroupId { get; init; }
    public int? RegisteredLocationId { get; init; }
    public DateOnly? HireDate { get; init; }
    public string? CurrentLocation { get; init; }
    public MilitaryServiceStatus? MilitaryServiceStatus { get; init; }
    public bool? IsGraduateJobPlacement { get; init; }

    public string? PersonalEmail { get; init; }
    public string? PersonalEmailSecond { get; init; }
    public string? PersonalPhone { get; init; }
    public string? Telegram { get; init; }
    public string? LinkedIn { get; init; }
    public DateOnly? DateOfBirth { get; init; }
    public int? CreatedById { get; init; }
}
