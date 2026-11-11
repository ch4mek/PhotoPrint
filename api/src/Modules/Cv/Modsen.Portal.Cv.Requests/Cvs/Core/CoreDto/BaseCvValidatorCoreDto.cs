using Modsen.Portal.Cv.Requests.Cvs.Requests.RequestDto;

namespace Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;

internal abstract record BaseCvValidatorCoreDto
{
    public required string Name { get; set; }
    public int UserId { get; set; }
    public List<CreateWorkExperienceDto> WorkExperiences { get; set; } = [];
    public List<CreateLanguageDto> Languages { get; set; } = [];
    public List<CreateSkillGroupDto> SkillGroups { get; set; } = [];
}
