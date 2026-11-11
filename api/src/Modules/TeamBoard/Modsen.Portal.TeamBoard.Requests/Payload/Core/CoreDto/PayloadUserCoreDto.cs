namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

public class PayloadUserCoreDto
{
    public int UserId { get; set; }
    public bool IsActive { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public string? Color { get; set; }
    public string? SpecializationName { get; set; }
    public string? SpecializationSeniority { get; set; }
    public string? DepartmentId { get; set; }
    public int? ResourceGroupId { get; set; }
    public string? ResourceGroupName { get; set; }
    public string? Comments { get; set; }
    public string? Plan { get; set; }
    public bool NeedProject { get; set; }
    public IReadOnlyCollection<PayloadAssignedProject> Projects { get; set; } = [];
    public IReadOnlyCollection<PayloadUserHoursCoreDto> HourDetails { get; set; } = [];
    public bool IsHeadOfDepartment { get; set; }
    public bool HasContracts { get; set; }
    public DateOnly? HireDate { get; set; }
    public bool CanUserInfoBeRetrievedByCurrentUser { get; set; }
}
