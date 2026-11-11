namespace Modsen.Portal.TeamBoard.Requests.Skills.Core.CoreDto;

public class HardSkillDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? SeniorityLevelId { get; set; }
    public string? SeniorityLevelName { get; set; }
    public DateTime? AddedAtUtc { get; set; }
    public bool? IsActive { get; set; }
}
