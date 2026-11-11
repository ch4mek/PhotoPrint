namespace Modsen.Portal.TeamBoard.Requests.Home.Requests.RequestsDto;

internal sealed record UserBirthdayDto
{
    public required string LastName { get; init; }
    public required string FirstName { get; init; }
    public string? AvatarFileId { get; init; }
    public string? Department { get; set; }

}
