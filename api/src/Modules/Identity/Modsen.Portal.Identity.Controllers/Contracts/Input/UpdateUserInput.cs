using Modsen.Portal.Core;

namespace Modsen.Portal.Identity.Controllers.Contracts.Input;

public class UpdateUserInput
{
    public required int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public string? PatronymicRu { get; set; }
    public string? Email { get; set; }
    public string? Color { get; set; }
    public int? CountryId { get; init; }
    public int? RegisteredLocationId { get; init; }
    public string? CurrentLocation { get; set; }
    public required string Position { get; set; }
    public LocationType? Location { get; set; }
    public string? DepartmentId { get; set; }
    public int? ResourceGroupId { get; set; }
    public DateOnly? HireDate { get; set; }
}
