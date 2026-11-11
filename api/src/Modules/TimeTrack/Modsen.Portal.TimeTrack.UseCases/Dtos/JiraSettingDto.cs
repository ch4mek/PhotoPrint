using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TimeTrack.UseCases.Dtos;

public class JiraSettingDto
{
    public int Id { get; set; }
    public required string Url { get; set; }
    public required string JiraProjectTag { get; set; }
    public string? Email { get; set; }
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public string? ProjectName { get; set; }

    internal static JiraSettingDto Of(JiraSetting source)
    {
        return new JiraSettingDto
        {
            Id = source.Id,
            Url = source.Url,
            JiraProjectTag = source.JiraProjectTag,
            Email =  source.Email,
            ProjectId = source.ProjectId,
            UserId = source.UserId,
            ProjectName = source.Project?.Name
        };
    }
}
