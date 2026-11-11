using Modsen.Portal.Cv.Requests.Cvs.Requests.RequestDto;

namespace Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;

internal record CreateCvValidatorCoreDto : BaseCvValidatorCoreDto
{
    public static CreateCvValidatorCoreDto Of(CreateCvDto createCvDto)
    {
        return new CreateCvValidatorCoreDto
        {
            Name = createCvDto.CvName,
            UserId = createCvDto.UserId,
            WorkExperiences = createCvDto.WorkExperiences,
            Languages = createCvDto.Languages,
            SkillGroups = createCvDto.SkillGroups
        };
    }
}
