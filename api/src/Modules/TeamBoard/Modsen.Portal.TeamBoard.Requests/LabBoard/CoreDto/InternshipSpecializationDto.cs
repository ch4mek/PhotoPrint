using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.LabBoard.CoreDto;

internal sealed class InternshipSpecializationDto : BaseSpecializationWithHardSkillsDto
{
    public new IReadOnlyCollection<InternsipHardSkillDto> HardSkills { get; set; } = [];

    internal static InternshipSpecializationDto Of(InternshipSpecialization source)
    {
        return new InternshipSpecializationDto
        {
            Id = source.Id,
            SeniorityLevelId = source.SeniorityLevelId,
            SeniorityLevelName = source.SeniorityLevel?.ShortName,
            SpecializationTemplateId = source.SpecializationTemplateId,
            SpecializationTemplateName = source.SpecializationTemplate!.Name,
            HardSkills = source.InternshipSpecializationHardSkills
                .Select(hs => InternsipHardSkillDto.Of(hs))
                .ToList()
        };
    }
}
