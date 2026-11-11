using Modsen.Portal.Cv.Requests.Cvs.Requests.RequestDto;

namespace Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;

internal record UpdateCvValidatorCoreDto : BaseCvValidatorCoreDto
{
    public int Id { get; set; }

    public static UpdateCvValidatorCoreDto Of(UpdateCvDto updateCvDto)
    {
        return new UpdateCvValidatorCoreDto
        {
            Id = updateCvDto.Id,
            Name = updateCvDto.CvName,
            UserId = updateCvDto.UserId,
            WorkExperiences = updateCvDto.WorkExperiences,
            Languages = updateCvDto.Languages,
            SkillGroups = updateCvDto.SkillGroups
        };
    }
}
