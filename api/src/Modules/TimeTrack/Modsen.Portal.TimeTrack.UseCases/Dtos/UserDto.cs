using Modsen.Portal.TimeTrack.Domain;

namespace Modsen.Portal.TimeTrack.UseCases.Dtos;

internal class UserDto
{
    public int UserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Color { get; set; }
    public string? AvatarFileId { get; set; }

    public static UserDto Of(TimeTrackUser source)
    {
        return new UserDto
        {
            UserId = source.UserId,
            FirstName = source.FirstName,
            LastName = source.LastName,
            Color = source.Color,
            AvatarFileId = source.AvatarFileId,
        };
    }
}
