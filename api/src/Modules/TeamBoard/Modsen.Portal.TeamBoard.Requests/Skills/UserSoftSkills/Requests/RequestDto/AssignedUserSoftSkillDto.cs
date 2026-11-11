namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Requests.RequestDto;

internal class AssignedUserSoftSkillDto
{
    public int SoftSkillId { get; set; }
    public required string SoftSkillName { get; set; }
    public string? SeniorityLevelId { get; set; }
    public string? SeniorityLevelName { get; set; }
    public int? SoftSkillSeniorityOrderWeight { get; set; }
    public DateOnly? SnapshotDate { get; set; }
}
