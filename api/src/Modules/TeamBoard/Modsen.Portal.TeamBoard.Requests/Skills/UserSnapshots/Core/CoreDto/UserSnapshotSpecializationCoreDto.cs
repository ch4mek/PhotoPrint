using Modsen.Portal.TeamBoard.Requests.Skills.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Core.CoreDto;

public class UserSnapshotSpecializationCoreDto
{
    public int Id { get; set; }
    public string? SeniorityLevelId { get; set; }
    public string? SeniorityLevelName { get; set; }
    public bool WasPrimary { get; set; }
    public int SpecializationTemplateId { get; set; }
    public required string SpecializationTemplateName { get; set; }
    public DateTime? CreatedAtUtc { get; set; }
    public bool IsPrimary { get; set; }
    public IReadOnlyCollection<HardSkillDto> HardSkills { get; set; } = [];
}
