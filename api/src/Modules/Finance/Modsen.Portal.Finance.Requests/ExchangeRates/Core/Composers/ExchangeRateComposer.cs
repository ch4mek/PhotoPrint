using Microsoft.Extensions.Logging;
using Modsen.Portal.BcseApi.Contract.Models;
using Modsen.Portal.Core.Finance;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Finance.Requests.ExchangeRates.Core.Composers;

internal interface IExchangeRateComposer
{
    Task Compose(ExchangeRatesByDateModel exchangeRateModel, string ComposeToCurrencyId, CancellationToken cancellationToken, bool saveChanges = false);
}

internal class ExchangeRateComposer : IExchangeRateComposer
{
    private readonly IFinanceDbContext _dbContext;
    private readonly ILogger<ExchangeRateComposer> _logger;

    public ExchangeRateComposer(IFinanceDbContext dbContext, ILogger<ExchangeRateComposer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public Task Compose(ExchangeRatesByDateModel exchangeRateModel, string ComposeToCurrencyId, CancellationToken cancellationToken, bool saveChanges = false)
    {
        foreach (var currentExchangeRate in exchangeRateModel.ExchangeRates)
        {
            if (!CurrencyCodes.ToList().Contains(currentExchangeRate.CurrencyCode))
            {
                _logger.LogWarning("Bcse API returned a new currency with a code: {CurrencyCode}", currentExchangeRate.CurrencyCode);

                continue;
            }

            _dbContext.ExchangeRates.Add(new ExchangeRateEntity
            {
                FromCurrencyId = currentExchangeRate.CurrencyCode,
                ToCurrencyId = ComposeToCurrencyId,
                CurrencyMultiplier = currentExchangeRate.RateCoef,
                Rate = currentExchangeRate.RateNB,
                ExchangeDate = exchangeRateModel.Date
            });
        }

        if (saveChanges)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        return Task.CompletedTask;
    }
}
