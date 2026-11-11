using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

internal sealed record UpdateSpecializationTemplateInput(
    int Id,
    string Name,
    IReadOnlyCollection<string> DepartmentIds,
    IReadOnlyCollection<SpecializationTemplateHardSkillDto>? HardSkills)
{
    public UpdateSpecializationTemplateDto ToRequestDto()
    {
        return new UpdateSpecializationTemplateDto(
            Id: Id,
            Name: Name,
            DepartmentIds: DepartmentIds,
            HardSkills: HardSkills);
    }
}
