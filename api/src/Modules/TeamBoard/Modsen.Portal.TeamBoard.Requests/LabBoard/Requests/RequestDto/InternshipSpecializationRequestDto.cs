using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.LabBoard.Requests.RequestDto;

internal record InternshipSpecializationRequestDto(
    int? Id,
    string? SeniorityLevelId,
    int SpecializationTemplateId,
    IEnumerable<HardSkillSeniorityDto> HardSkills);
