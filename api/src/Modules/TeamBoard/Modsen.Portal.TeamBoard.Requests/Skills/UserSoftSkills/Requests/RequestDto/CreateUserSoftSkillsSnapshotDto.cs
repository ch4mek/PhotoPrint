using FluentValidation;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Requests.RequestDto;

internal sealed record CreateUserSoftSkillsSnapshotDto(
    int UserId,
    DateOnly SnapshotDate,
    List<UserSoftSkillSeniorityLevelDto> SkillsSeniorityLevels)
{
    internal sealed class Validator : AbstractValidator<CreateUserSoftSkillsSnapshotDto>
    {
        public Validator()
        {
            RuleFor(x => x.SnapshotDate)
                .Must(x => x <= DateOnly.FromDateTime(DateTime.UtcNow));
        }
    }
}
