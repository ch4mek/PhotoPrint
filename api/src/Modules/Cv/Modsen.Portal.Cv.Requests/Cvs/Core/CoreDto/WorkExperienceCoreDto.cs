using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;

internal record WorkExperienceCoreDto
{
    public int Id { get; set; }
    public required string Specialization { get; set; }
    public int ExperienceInYears { get; set; }
    public int CvId { get; set; }

    public static WorkExperienceCoreDto Of(WorkExperienceEntity entity)
    {
        return new WorkExperienceCoreDto
        {
            Id = entity.Id,
            Specialization = entity.Specialization,
            ExperienceInYears = entity.ExperienceInYears,
            CvId = entity.CvId,
        };
    }
}
