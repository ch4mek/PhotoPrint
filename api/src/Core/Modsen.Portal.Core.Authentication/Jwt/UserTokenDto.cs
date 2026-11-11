using System.Security.Claims;

namespace Modsen.Portal.Core.Authentication.Jwt;

public sealed class UserTokenDto
{
    public UserTokenDto(string nameIdentifier, string email, string firstName, string lastName, int? resourceGroupId)
    {
        NameIdentifier = nameIdentifier;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        ResourceGroupId = resourceGroupId;
    }

    public string NameIdentifier { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? DepartmentId { get; set; }
    public int? ResourceGroupId { get; set; }
    public IReadOnlyCollection<string> Roles { get; set; } = [];
    public IReadOnlyCollection<Claim> Claims { get; set; } = [];
}
