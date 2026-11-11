namespace Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Core.CoreDto;

internal record UserSoftSkillsSnapshotCoreDto(
    IReadOnlyCollection<UserSoftSkillCoreDto> UserSoftSkills,
    DateOnly SnapshotDate,
    int SnapShotId);
