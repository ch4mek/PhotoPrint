using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.Skills.Core.BusinessRules;

internal static class HardSkillBusinessRules
{
    public static bool IsActiveHardSkill(UserSkillsSnapshotSpecializationHardSkill hs)
    {
        return hs.HardSkill!.IsActive && IsHardSkillAssignedToSpecializationTemplate(hs);
    }

    public static bool ShouldIncludeHardSkill(UserSkillsSnapshotSpecializationHardSkill hs)
    {
        var isActive = IsActiveHardSkill(hs);
        return isActive || (!isActive && hs.SeniorityLevelId != null);
    }

    public static bool IsHardSkillAssignedToSpecializationTemplate(UserSkillsSnapshotSpecializationHardSkill snapshot)
    {
        return snapshot.HardSkill!.SpecializationTemplateHardSkills.Any(x =>
            x.HardSkillId == snapshot.HardSkillId &&
            x.SpecializationTemplateId == snapshot.UserSkillsSnapshotSpecialization!.SpecializationTemplateId);
    }
}
