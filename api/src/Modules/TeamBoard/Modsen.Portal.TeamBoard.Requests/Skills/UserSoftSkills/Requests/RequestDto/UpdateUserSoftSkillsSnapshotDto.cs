namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Requests.RequestDto;

internal sealed record UpdateUserSoftSkillsSnapshotDto(
    int SnapshotId,
    IReadOnlyCollection<UserSoftSkillSeniorityLevelDto> SkillsSeniorityLevels);
