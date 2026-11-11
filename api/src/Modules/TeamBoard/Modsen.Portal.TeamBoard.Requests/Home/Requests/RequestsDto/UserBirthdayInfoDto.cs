namespace Modsen.Portal.TeamBoard.Requests.Home.Requests.RequestsDto;

internal sealed record UserBirthdayInfoDto
{
    public required int UserId { get; set; }
    public required string Email { get; set; }
    public required string FullName { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? FullNameRu { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public string? AvatarFileId { get; init; }
    public string? Department { get; set; }
}
