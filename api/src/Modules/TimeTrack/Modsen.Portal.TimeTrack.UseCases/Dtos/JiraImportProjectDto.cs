namespace Modsen.Portal.TimeTrack.UseCases.Dtos;

public sealed class JiraImportProjectDto
{
    public int JiraSettingId { get; set; }
    public int ProjectId { get; set; }
    public required string ProjectName { get; set; }
    public required string JiraProjectTag { get; set; }
}
