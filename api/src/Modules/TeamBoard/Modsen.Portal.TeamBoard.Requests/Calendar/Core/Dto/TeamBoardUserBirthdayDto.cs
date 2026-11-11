namespace Modsen.Portal.TeamBoard.Requests.Calendar.Core.Dto;

internal sealed class TeamBoardUserBirthdayDto
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; } 
    public required string Email { get; set; }
    public string? AvatarFileId { get; set; }
    public int WorkedFullYears { get; set; }
}
