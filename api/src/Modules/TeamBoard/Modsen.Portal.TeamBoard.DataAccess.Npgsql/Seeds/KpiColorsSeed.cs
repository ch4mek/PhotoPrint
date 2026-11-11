using Modsen.Portal.Core.Database;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.Seeds;

internal class KpiColorsSeed : ISeedEntitiesProvider<TeamBoardDbContext>
{
    private readonly TeamBoardDbContext _dbContext;

    public KpiColorsSeed(TeamBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Seed()
    {
        foreach (var kpiColor in GetCurrencies())
        {
            if (!_dbContext.KpiColors.Any(x => x.Id == kpiColor.Id))
            {
                _dbContext.KpiColors.Add(kpiColor);
            }
        }

        _dbContext.SaveChanges();
    }

    private static IEnumerable<KpiColor> GetCurrencies()
    {
        yield return new KpiColor {Id = 1, Name = "Red", RightIntervalBorder = 30, ColorOrderWeight = 100};
        yield return new KpiColor {Id = 2, Name = "Orange", RightIntervalBorder = 50, ColorOrderWeight = 200};
        yield return new KpiColor {Id = 3, Name = "Green", RightIntervalBorder = 9999, ColorOrderWeight = 300};
    }
}
