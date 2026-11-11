namespace Modsen.Portal.Core.TeamBoard;

[Flags]
public enum SeniorityLevelScopes
{
    None = 0,
    SoftSkill = 1 << 0,
    HardSkill = 1 << 1,
    ProjectUser = 1 << 2,
    TeamBoardUser = 1 << 3,
    UserSpecialization = 1 << 4,
    GeneralSeniority = 1 << 5
}

