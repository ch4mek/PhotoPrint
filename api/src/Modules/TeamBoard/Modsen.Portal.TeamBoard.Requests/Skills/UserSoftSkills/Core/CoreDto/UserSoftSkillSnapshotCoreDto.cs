using Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Core.CoreDto;

internal sealed record UserSoftSkillSnapshotCoreDto(
    int UserId,
    DateOnly SnapshotDate,
    ICollection<UserSoftSkillSeniorityLevelCoreDto> UserSoftSkillCoreDtos)
{
    public static UserSoftSkillSnapshotCoreDto Of(CreateUserSoftSkillsSnapshotDto source)
    {
        return new UserSoftSkillSnapshotCoreDto(
            UserId: source.UserId,
            SnapshotDate: source.SnapshotDate,
            UserSoftSkillCoreDtos: source.SkillsSeniorityLevels
                .Select(seniorityLevelDto => new UserSoftSkillSeniorityLevelCoreDto(
                        seniorityLevelDto.SoftSkillId,
                        seniorityLevelDto.SeniorityLevelId)).ToList());
    }
}
