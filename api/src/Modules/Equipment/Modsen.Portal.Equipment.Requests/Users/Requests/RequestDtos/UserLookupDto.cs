namespace Modsen.Portal.Equipment.Requests.Users.Requests.RequestDtos;

internal sealed class UserLookupDto
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? PositionDescription { get; set; }
    public int? ResourceGroupId { get; set; }
    public string? DepartmentId { get; set; }
}
