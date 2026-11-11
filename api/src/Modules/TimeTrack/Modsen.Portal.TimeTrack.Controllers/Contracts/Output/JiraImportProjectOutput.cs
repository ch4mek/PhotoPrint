using Modsen.Portal.TimeTrack.UseCases.Dtos;

namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Output;

public sealed class JiraImportProjectOutput
{
    public int JiraSettingId { get; set; }
    public int ProjectId { get; set; }
    public required string ProjectName { get; set; }
    public required string JiraProjectTag { get; set; }

    internal static JiraImportProjectOutput Of(JiraImportProjectDto source)
    {
        return new JiraImportProjectOutput
        {
            JiraSettingId = source.JiraSettingId,
            ProjectId = source.ProjectId,
            ProjectName = source.ProjectName,
            JiraProjectTag = source.JiraProjectTag
        };
    }
}
