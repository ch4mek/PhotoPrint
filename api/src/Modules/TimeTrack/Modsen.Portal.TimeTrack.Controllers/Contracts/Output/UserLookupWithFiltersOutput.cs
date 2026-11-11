using Modsen.Portal.TimeTrack.UseCases.Dtos;

namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Output;

internal class UserLookupWithFiltersOutput
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Color { get; set; }
    public string? AvatarFileId { get; set; }

    public static UserLookupWithFiltersOutput Of(UserDto source)
    {
        return new UserLookupWithFiltersOutput
        {
            Id = source.UserId,
            FirstName = source.FirstName,
            LastName = source.LastName,
            Color = source.Color,
            AvatarFileId = source.AvatarFileId,
        };
    }
}
