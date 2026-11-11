namespace Modsen.Portal.Merch.Requests.Users.RequestDto;

public class MerchUserLookupDto
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? DepartmentId { get; set; }
}
