namespace Modsen.Portal.Inquiries.Requests.Assigns.Requests.RequestDtos;

public sealed class TemplateUsersLookupDto
{
    public IReadOnlyCollection<UserLookupDto> AssignedUsers { get; set; } = [];
    public IReadOnlyCollection<UserLookupDto> UnassignedUsers { get; set; } = [];
}

public sealed record UserLookupDto(int Id, string FullName, string FirstName, string LastName);
