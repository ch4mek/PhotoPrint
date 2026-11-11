using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests.RequestDto;

internal class UserSpecializationTemplateWithHardSkillsDto : BaseSpecializationWithHardSkillsDto
{
    public bool IsPrimary { get; set; }
}
