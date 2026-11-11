namespace Modsen.Portal.Identity.Contract.Models;

public class ProjectManagerLookupDto
{
    public int UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Role { get; set; }
}
