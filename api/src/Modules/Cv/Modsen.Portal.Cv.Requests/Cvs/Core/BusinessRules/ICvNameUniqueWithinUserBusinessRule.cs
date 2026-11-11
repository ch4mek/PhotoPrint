using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Cv.DataAccess.Npgsql;

namespace Modsen.Portal.Cv.Requests.Cvs.Core.BusinessRules;

internal interface ICvNameUniqueWithinUserBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(string cvName, int userId, int? cvId, CancellationToken cancellationToken);
}

internal class CvNameUniqueWithinUserBusinessRule : ICvNameUniqueWithinUserBusinessRule
{
    private readonly ICvDbContext _cvDbContext;

    public CvNameUniqueWithinUserBusinessRule(ICvDbContext cvDbContext)
    {
        _cvDbContext = cvDbContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(string cvName, int userId, int? cvId, CancellationToken cancellationToken)
    {
        return !await _cvDbContext.Cvs.AnyAsync(x =>
                x.CvName == cvName &&
                x.UserId == userId &&
                (!cvId.HasValue || x.Id != cvId),
            cancellationToken);
    }
}
