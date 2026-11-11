namespace Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests.RequestDto;

internal class SpecializationTemplateWithDepartmentsAndHardSkillsDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public IReadOnlyCollection<SpecializationTemplateHardSkillDto> HardSkills { get; set; } = [];
    public IReadOnlyCollection<string> DepartmentIds { get; set; } = [];
}
