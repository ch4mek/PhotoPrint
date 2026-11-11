namespace Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Requests.RequestDto;

internal sealed class HardSkillDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string HardSkillGroupId { get; set; }
    public string? HardSkillGroupName { get; set; }
}
