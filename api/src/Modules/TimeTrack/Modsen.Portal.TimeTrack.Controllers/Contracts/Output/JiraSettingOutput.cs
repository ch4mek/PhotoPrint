using Modsen.Portal.TimeTrack.UseCases.Dtos;

namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Output;

public sealed class JiraSettingOutput
{
    public int Id { get; set; }
    public required string Url { get; set; }
    public required string JiraProjectTag { get; set; }
    public string? Email { get; set; }
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public string? ProjectName { get; set; }

    internal static JiraSettingOutput Of(JiraSettingDto source)
    {
        return new JiraSettingOutput
        {
            Id = source.Id,
            Url = source.Url,
            JiraProjectTag = source.JiraProjectTag,
            Email  = source.Email,
            ProjectId = source.ProjectId,
            UserId = source.UserId,
            ProjectName = source.ProjectName
        };
    }
}
