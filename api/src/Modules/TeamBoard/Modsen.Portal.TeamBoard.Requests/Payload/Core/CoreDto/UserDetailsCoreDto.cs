namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

internal class UserDetailsCoreDto
{
    public int UserId { get; set; }
    public bool IsActive { get; set; }
    public string? Comment { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Color { get; set; }
    public bool IsHeadOfDepartment { get; set; }
    public bool NeedProject { get; set; }
    public string? PrimarySpecializationName { get; set; }
    public string? DepartmentId { get; set; }
    public int? ResourceGroupId { get; set; }
    public string? ResourceGroupName { get; set; }
    public IReadOnlyCollection<RegisteredLocationHistoryCoreDto> RegisteredLocations { get; set; } = [];
    public string? SpecializationSeniority { get; set; }
    public IReadOnlyCollection<UserProjectDetailsCoreDto> ProjectDetails { get; set; } = [];
    public IReadOnlyCollection<UserMonthPayloadDetailsCoreDto> UserMonthPayloadDetails { get; set; } = [];
    public string? Plan { get; set; }
    public List<ContractDatesDto> ContractDates { get; set; } = [];
    public bool IsUserHasContracts { get; set; }
    public DateOnly? HireDate { get; set; }

    public DateOnly CareerStartedAt { get; set; }
    public IReadOnlyCollection<UserDepartmentHistoryCoreDto> UserDepartmentHistory { get; set; } = [];
}
