using Modsen.Portal.Core.Cv;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;

internal record SkillCoreDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public CvSkillLevel? Level { get; set; }
    public int? ExperienceInYears { get; set; }
    public int? LastUsedYear { get; set; }
    public int OrderWeight { get; set; }
    public int SkillGroupId { get; set; }

    public static SkillCoreDto Of(SkillEntity entity)
    {
        return new SkillCoreDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Level = entity.Level,
            ExperienceInYears = entity.ExperienceInYears,
            LastUsedYear = entity.LastUsedYear,
            OrderWeight = entity.OrderWeight,
            SkillGroupId = entity.SkillGroupId,
        };
    }
}
