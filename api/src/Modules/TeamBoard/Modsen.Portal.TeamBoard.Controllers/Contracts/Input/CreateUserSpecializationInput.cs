using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

public record CreateUserSpecializationInput(
    int UserId,
    int SpecializationTemplateId,
    string SpecializationSeniorityLevelId,
    IReadOnlyCollection<HardSkillSeniorityDto> HardSkills);
