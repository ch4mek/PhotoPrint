using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests.RequestDto;

public class CreateUserSpecializationDto
{
    public int SpecializationTemplateId { get; set; }
    public string? SpecializationSeniorityLevelId { get; set; }
    public IReadOnlyCollection<HardSkillSeniorityDto> HardSkills { get; set; } = [];
}
