using Modsen.Portal.Core.Database;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.Seeds;

internal class EnglishLevelsSeed : ISeedEntitiesProvider<TeamBoardDbContext>
{
    private readonly TeamBoardDbContext _dbContext;

    public EnglishLevelsSeed(TeamBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Seed()
    {
        foreach (var englishLevel in GetEnglishLevels())
        {
            if (!_dbContext.EnglishLevels.Any(x => x.Id == englishLevel.Id))
            {
                _dbContext.EnglishLevels.Add(englishLevel);
            }
        }

        _dbContext.SaveChanges();
    }

    private static IEnumerable<EnglishLevel> GetEnglishLevels()
    {
        yield return Create("A1", "A1", "A1", "Elementary", 100);
        yield return Create("A2", "A2", "A2", "Pre-Intermediate", 200);

        yield return Create("B1L1", "B1", "B1", "Intermediate", 230);
        yield return Create("B1L2", "B1", "B1+", "Intermediate", 300);
        yield return Create("B2L1", "B2", "B2", "Upper-Intermediate", 350);

        yield return Create("C1", "C1", "C1", "Advanced", 500);
        yield return Create("C2", "C2", "C2", "Proficient", 600);
    }

    private static EnglishLevel Create(string id, string shortName, string internalName, string externalName, int orderWeight)
    {
        return new EnglishLevel
        {
            Id = id,
            ShortName = shortName,
            InternalName = internalName,
            ExternalName = externalName,
            Description = externalName,
            OrderWeight = orderWeight
        };
    }
}
