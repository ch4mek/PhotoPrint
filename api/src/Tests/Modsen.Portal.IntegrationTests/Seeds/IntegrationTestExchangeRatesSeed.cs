using Modsen.Portal.Core.Database;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.IntegrationTests.Seeds;

internal class IntegrationTestExchangeRatesSeed : ISeedEntitiesProvider<FinanceDbContext>
{
    private readonly FinanceDbContext _dbContext;

    public IntegrationTestExchangeRatesSeed(FinanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Seed()
    {
        var exchangeRates = new[]
        {
            GetExchangeRateEntity("USD", "BYN", 1, 2.880500M, new DateOnly(2026, 7, 27))
        };

        _dbContext.ExchangeRates.AddRange(exchangeRates);
        _dbContext.SaveChanges();
    }

    private static ExchangeRateEntity GetExchangeRateEntity(string from, string to, int multiplier, decimal rate, DateOnly date)
    {
        return new ExchangeRateEntity
        {
            FromCurrencyId = from,
            ToCurrencyId = to,
            CurrencyMultiplier = multiplier,
            Rate = rate,
            ExchangeDate = date
        };
    }
}
