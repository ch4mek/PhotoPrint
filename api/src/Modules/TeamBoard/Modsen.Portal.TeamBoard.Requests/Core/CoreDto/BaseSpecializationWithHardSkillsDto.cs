namespace Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

internal class BaseSpecializationWithHardSkillsDto
{
    public int Id { get; set; }
    public string? SeniorityLevelId { get; set; }
    public string? SeniorityLevelName { get; set; }
    public int SpecializationTemplateId { get; set; }
    public required string SpecializationTemplateName { get; set; }
    public IReadOnlyCollection<ShortHardSkillDto> HardSkills { get; set; } = [];
}
