using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;

internal record SkillGroupCoreDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string NameRu { get; set; }
    public bool IsCustom { get; set; }
    public int OrderWeight { get; set; }
    public List<SkillCoreDto> Skills { get; set; } = [];
    public int CvId { get; set; }

    public static SkillGroupCoreDto Of(SkillGroupEntity entity)
    {
        return new SkillGroupCoreDto
        {
            Id = entity.Id,
            Name = entity.Name,
            NameRu = entity.NameRu,
            IsCustom = entity.IsCustom,
            OrderWeight = entity.OrderWeight,
            Skills = entity.Skills.Select(SkillCoreDto.Of).ToList(),
            CvId = entity.CvId,
        };
    }
}
