using Modsen.Portal.Core.Database;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.Seeds;

internal class CountriesSeed : ISeedEntitiesProvider<TeamBoardDbContext>
{
    private readonly TeamBoardDbContext _teamBoardDbContext;

    public CountriesSeed(TeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public void Seed()
    {
        foreach (var country in GetCountries())
        {
            if (!_teamBoardDbContext.Countries.Any(x => x.Name == country.Name))
            {
                _teamBoardDbContext.Countries.Add(country);
            }
        }

        _teamBoardDbContext.SaveChanges();
    }

    private static IEnumerable<Country> GetCountries()
    {
        yield return CreateCountry("Belarus");
        yield return CreateCountry("Poland");
    }

    private static Country CreateCountry(string name)
    {
        return new Country { Name = name };
    }
}
