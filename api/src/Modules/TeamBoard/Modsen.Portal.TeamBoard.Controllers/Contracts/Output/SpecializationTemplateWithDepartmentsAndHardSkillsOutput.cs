using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Output;

internal class SpecializationTemplateWithDepartmentsAndHardSkillsOutput
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public IReadOnlyCollection<SpecializationTemplateHardSkillDto> HardSkills { get; set; } = [];
    public IReadOnlyCollection<SpecializationTemplateDepartmentDto> DepartmentDtos { get; set; } = [];
}
