using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Cv.Requests.Cvs.Core.DbQueries;

internal interface IGetCvByIdWithIncludesDbQuery
{
    Task<CvEntity?> ExecuteAsync(int id, CancellationToken cancellationToken);
}

internal class GetCvByIdWithIncludesDbQuery : IGetCvByIdWithIncludesDbQuery
{
    private readonly ICvDbContext _cvDbContext;

    public GetCvByIdWithIncludesDbQuery(ICvDbContext cvDbContext)
    {
        _cvDbContext = cvDbContext;
    }

    public async Task<CvEntity?> ExecuteAsync(int id, CancellationToken cancellationToken)
    {
        return await _cvDbContext.Cvs
            .Include(x => x.WorkExperiences)
            .Include(x => x.Languages)
            .Include(x => x.ProjectExperiences)
            .Include(x => x.SkillGroups)
            .ThenInclude(x => x.Skills)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }
}
