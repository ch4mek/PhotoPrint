namespace Modsen.Portal.Identity.Contract.Models;

public class UserShortDto
{
    public required int UserId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string? Email { get; init; }
    public required string? AvatarFileId { get; init; }
    public required bool IsActive { get; init; }
}
