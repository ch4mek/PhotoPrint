using Modsen.Portal.Core.Cv;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;

internal record LanguageCoreDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public CvLanguageLevel Level { get; set; }
    public int CvId { get; set; }

    public static LanguageCoreDto Of(LanguageEntity entity)
    {
        return new LanguageCoreDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Level = entity.Level,
            CvId = entity.CvId,
        };
    }
}
