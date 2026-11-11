namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Output;

internal class AssignedSoftSkillOutput
{
    public int SoftSkillId { get; set; }
    public required string SoftSkillName { get; set; }
    public string? SeniorityLevelId { get; set; }
    public string? SeniorityLevelName { get; set; }
    public int? SoftSkillSeniorityOrderWeight { get; set; }
    public DateOnly? SnapshotDate { get; set; }
}
