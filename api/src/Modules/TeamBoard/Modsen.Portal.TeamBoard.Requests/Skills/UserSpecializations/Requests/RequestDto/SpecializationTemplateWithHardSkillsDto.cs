using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests.RequestDto;

internal class SpecializationTemplateWithHardSkillsDto
{
    public int SpecializationTemplateId { get; set; }
    public required string SpecializationTemplateName { get; set; }
    public IReadOnlyCollection<ShortHardSkillDto> HardSkills { get; set; } = [];

    public static SpecializationTemplateWithHardSkillsDto Of(SpecializationTemplate source)
    {
        return new SpecializationTemplateWithHardSkillsDto
        {
            SpecializationTemplateId = source.Id,
            SpecializationTemplateName = source.Name,
            HardSkills = source.SpecializationTemplateHardSkills
                .OrderByDescending(hs => hs.CreatedAtUtc)
                .ThenByDescending(hs => hs.HardSkillId)
                .Select(hs => new ShortHardSkillDto
                {
                        Id = hs.HardSkillId,
                        Name = hs.HardSkill!.Name
                    })
                .ToArray()
        };
    }
}
