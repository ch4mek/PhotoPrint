using Modsen.Portal.Core;
using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.Identity.Controllers.Contracts.Input;

public class CreateUserInput
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string FirstNameRu { get; set; }
    public required string LastNameRu { get; set; }
    public required string PatronymicRu { get; set; }
    public string? Color { get; set; }
    public string? Email { get; set; }
    public required string Position { get; set; }
    public required LocationType Location { get; set; }
    public string? DepartmentId { get; set; }
    public int? ResourceGroupId { get; set; }
    public int? CountryId { get; set; }
    public string? CurrentLocation { get; set; }
    public int RegisteredLocationId { get; set; }
    public DateOnly HireDate { get; set; }
    public MilitaryServiceStatus? MilitaryServiceStatus { get; set; }
    public bool? IsGraduateJobPlacement { get; set; }
    public DateOnly? ContractDateFrom { get; set; }
    public DateOnly? ContractDateTo { get; set; }

    public string? PersonalEmail { get; set; }
    public string? PersonalEmailSecond { get; set; }
    public string? PersonalPhone { get; set; }
    public string? Telegram { get; set; }
    public string? LinkedIn { get; set; }
    public DateOnly DateOfBirth { get; set; }
}
