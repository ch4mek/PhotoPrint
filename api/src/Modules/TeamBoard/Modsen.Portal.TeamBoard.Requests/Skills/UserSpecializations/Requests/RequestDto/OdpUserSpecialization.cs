using Modsen.Portal.TeamBoard.Requests.Skills.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests.RequestDto;

public class OdpUserSpecialization
{
    public int OdpUserSpecializationId { get; set; }

    public int? SpecializationTemplateId { get; set; }
    public string? SpecializationTemplateName { get; set; }
    public bool IsPrimary { get; set; }
    public string? SeniorityLevelId { get; set; }
    public string? SeniorityLevelName { get; set; }
    public int? SeniorityLevelOrderWeight { get; set; }
    public IReadOnlyCollection<HardSkillDto> HardSkills { get; set; } = [];
}
