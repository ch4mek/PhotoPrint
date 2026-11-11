using Modsen.Portal.Core.Database;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.Seeds;

internal class CurrenciesSeed : ISeedEntitiesProvider<TeamBoardDbContext>
{
    private readonly TeamBoardDbContext _dbContext;

    public CurrenciesSeed(TeamBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Seed()
    {
        foreach (var currency in GetCurrencies())
        {
            if (!_dbContext.Currencies.Any(x => x.Id == currency.Id))
            {
                _dbContext.Currencies.Add(currency);
            }
        }

        _dbContext.SaveChanges();
    }

    private static IEnumerable<Currency> GetCurrencies()
    {
        yield return CreateCurrency("USD", "United States dollar", "USD");
        yield return CreateCurrency("EUR", "Euro", "EUR");
        yield return CreateCurrency("BYN", "Belarusian ruble", "BYN");
        yield return CreateCurrency("PLN", "Polish złoty", "PLN");
    }

    private static Currency CreateCurrency(string id, string name, string code)
    {
        return new Currency { Id = id, Name = name, CurrencyCode = code };
    }
}
