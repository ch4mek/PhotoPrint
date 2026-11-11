using Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

internal class CreateUserSoftSkillsSnapshotInput
{
    public DateOnly SnapshotDate { get; set; }
    public IReadOnlyCollection<UpdateUserSoftSkillInput> UserSoftSkills { get; set; } = [];

    public CreateUserSoftSkillsSnapshotDto ToRequestDto(int userId)
    {
        return new CreateUserSoftSkillsSnapshotDto(
            UserId: userId,
            SnapshotDate: SnapshotDate,
            SkillsSeniorityLevels: UserSoftSkills
                .Select(x => new UserSoftSkillSeniorityLevelDto(
                    SeniorityLevelId: x.SeniorityLevelId,
                    SoftSkillId: x.SoftSkillId)
                ).ToList());
    }
}

internal class UpdateUserSoftSkillsSnapshotInput
{
    public IReadOnlyCollection<UpdateUserSoftSkillInput> UserSoftSkills { get; set; } = [];

    public UpdateUserSoftSkillsSnapshotDto ToRequestDto(int snapshotId)
    {
        return new UpdateUserSoftSkillsSnapshotDto(
            SnapshotId: snapshotId,
            SkillsSeniorityLevels: UserSoftSkills
                .Select(x => new UserSoftSkillSeniorityLevelDto(
                        SeniorityLevelId: x.SeniorityLevelId,
                        SoftSkillId: x.SoftSkillId)
                ).ToList());
    }
}
