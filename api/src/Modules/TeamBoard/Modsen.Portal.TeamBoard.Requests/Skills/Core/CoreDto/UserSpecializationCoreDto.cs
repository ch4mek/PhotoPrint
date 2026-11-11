namespace Modsen.Portal.TeamBoard.Requests.Skills.Core.CoreDto;

public class UserSpecializationCoreDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? SeniorityLevelId { get; set; }
    public string? SeniorityLevelName { get; set; }
    public bool IsPrimary { get; set; }
    public int SpecializationTemplateId { get; set; }
    public required string SpecializationTemplateName { get; set; }
    public IReadOnlyCollection<HardSkillDto> HardSkills { get; set; } = [];
}
