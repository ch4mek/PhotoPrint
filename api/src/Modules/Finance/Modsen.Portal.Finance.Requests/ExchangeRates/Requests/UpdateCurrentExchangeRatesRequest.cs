using MediatR;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.BcseApi.Contract;
using Modsen.Portal.BcseApi.Contract.Models;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Finance;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Finance.Requests.ExchangeRates.Core.Composers;

namespace Modsen.Portal.Finance.Requests.ExchangeRates.Requests;

internal sealed class UpdateCurrentExchangeRatesRequest : ICommandHandler<UpdateCurrentExchangeRatesRequest.Command, Unit>
{
    private readonly IBcseApiClient _bcseApiClient;
    private readonly IFinanceDbContext _dbContext;
    private readonly IExchangeRateComposer _composer;

    public UpdateCurrentExchangeRatesRequest(
        IBcseApiClient bcseApiClient,
        IFinanceDbContext dbContext,
        IExchangeRateComposer composer)
    {
        _bcseApiClient = bcseApiClient;
        _dbContext = dbContext;
        _composer = composer;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        var currentExchangeRates = await _bcseApiClient.GetCurrentExchangeRates(cancellationToken);

        if (currentExchangeRates.ExchangeRates.Count == 0)
        {
            throw new AppException("There are no currency exchange rates for the current date.");
        }

        var latestDate = await _dbContext.ExchangeRates
            .AsNoTracking()
            .Select(x => x.ExchangeDate)
            .OrderByDescending(x => x)
            .FirstOrDefaultAsync(cancellationToken);

        if (latestDate == default)
        {
            await _composer.Compose(currentExchangeRates, CurrencyCodes.BelarusianRuble, cancellationToken, saveChanges: true);

            return Unit.Value;
        }

        if (latestDate == currentExchangeRates.Date)
        {
            return Unit.Value;
        }

        var currentExchangeRatesFromDb = await _dbContext.ExchangeRates
                .Where(x => x.ExchangeDate == latestDate)
                .ToListAsync(cancellationToken);

        if (latestDate.Month != currentExchangeRates.Date.Month || latestDate.Year != currentExchangeRates.Date.Year)
        {
            await _composer.Compose(currentExchangeRates, CurrencyCodes.BelarusianRuble, cancellationToken);

            if (latestDate.Day != DateTime.DaysInMonth(latestDate.Year, latestDate.Month))
            {
                await UpdateExchangeRatesForPreviousMonth(currentExchangeRatesFromDb, latestDate, cancellationToken);
            }
        }
        else
        {
            foreach (var exchangeRate in currentExchangeRatesFromDb)
            {
                UpdateExchangeRate(exchangeRate, currentExchangeRates);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private async Task UpdateExchangeRatesForPreviousMonth(List<ExchangeRateEntity> exchangeRates, DateOnly latestDate, CancellationToken cancellationToken)
    {
        var lastDayOfTheMonth = new DateOnly(latestDate.Year, latestDate.Month, DateTime.DaysInMonth(latestDate.Year, latestDate.Month));
        var date = lastDayOfTheMonth;

        while (date >= latestDate)
        {
            var result = await _bcseApiClient.GetExchangeRatesByDate(date, cancellationToken);

            if (result.ExchangeRates.Count > 0)
            {
                foreach (var exchangeRate in exchangeRates)
                {
                    UpdateExchangeRate(exchangeRate, result);

                    exchangeRate.ExchangeDate = lastDayOfTheMonth;
                }

                break;
            }

            date = date.AddDays(-1);
        }

        return;
    }

    private static void UpdateExchangeRate(ExchangeRateEntity exchangeRateEntity, ExchangeRatesByDateModel currentExchangeRates)
    {
        var currentExchangeRate = currentExchangeRates.ExchangeRates
            .FirstOrDefault(x => x.CurrencyCode == exchangeRateEntity.FromCurrencyId);

        if (currentExchangeRate is null)
        {
            return;
        }

        exchangeRateEntity.ExchangeDate = currentExchangeRates.Date;
        exchangeRateEntity.CurrencyMultiplier = currentExchangeRate.RateCoef;
        exchangeRateEntity.Rate = currentExchangeRate.RateNB;
    }

    public sealed record Command() : ICommand<Unit>, ITransactionalRequest;
}
