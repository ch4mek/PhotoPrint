using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

internal record CreateSpecializationTemplateInput(
    string Name,
    IReadOnlyCollection<string>? DepartmentIds,
    IReadOnlyCollection<int>? HardSkillids)
{
    public CreateSpecializationTemplateDto ToRequestDto()
    {
        return new CreateSpecializationTemplateDto(
            Name: Name,
            DepartmentIds: DepartmentIds,
            HardSkillIds: HardSkillids);
    }
}
