namespace Modsen.Portal.Identity.Contract.Models;

public class TokenPayloadDto
{
    public bool IsValidToken { get; set; }
    public bool IsTokenRevoked { get; set; }
    public string? UserId { get; set; }
    public string? UserEmail { get; set; }
    public string? UserFirstName { get; set; }
    public string? UserLastName { get; set; }
    public string? UserDepartmentId { get; set; }
    public string? UserResourceGroupId { get; set; }
    public string? UserRole { get; set; }

    public IReadOnlyCollection<ClaimDto> Claims { get; set; } = [];
}
