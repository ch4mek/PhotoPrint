namespace Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Core.CoreDto;

internal record UserSoftSkillCoreDto(
    int SoftSkillId,
    string SeniorityLevelId,
    int? SoftSkillSeniorityOrderWeight,
    int? SoftSkillSeniorityCoefficient,
    bool IsActive);
