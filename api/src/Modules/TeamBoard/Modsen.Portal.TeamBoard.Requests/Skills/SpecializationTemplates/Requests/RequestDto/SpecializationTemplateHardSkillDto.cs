using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests.RequestDto;

internal sealed class SpecializationTemplateHardSkillDto
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public static SpecializationTemplateHardSkillDto Of(SpecializationTemplateHardSkill source)
    {
        return new SpecializationTemplateHardSkillDto { Id = source.HardSkillId, Name = source.HardSkill!.Name };
    }
}
