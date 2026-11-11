using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.LabBoard.CoreDto;

internal class InternsipHardSkillDto : ShortHardSkillDto
{
    public string? SeniorityLevelId { get; set; }
    public string? SeniorityLevelName { get; set; }

    internal static InternsipHardSkillDto Of(InternshipSpecializationHardSkill source)
    {
        return new InternsipHardSkillDto
        {
            Id = source.HardSkillId,
            Name = source.HardSkill!.Name,
            SeniorityLevelId = source.SeniorityLevelId,
            SeniorityLevelName = source.SeniorityLevel?.ShortName
        };
    }
}
