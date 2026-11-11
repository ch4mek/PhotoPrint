namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Input;

public class CreateJiraSettingInput
{
    public required string Pat { get; set; }
    public required string Url { get; set; }
    public required string JiraProjectTag { get; set; }
    public string? Email { get; set; }
    public required int ProjectId { get; set; }
    public required int UserId { get; set; }
}
