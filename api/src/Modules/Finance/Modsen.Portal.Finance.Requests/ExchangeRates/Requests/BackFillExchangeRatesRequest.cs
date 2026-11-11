using MediatR;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.BcseApi.Contract;
using Modsen.Portal.BcseApi.Contract.Models;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Finance;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.Requests.ExchangeRates.Core.Composers;

namespace Modsen.Portal.Finance.Requests.ExchangeRates.Requests;

internal sealed class BackFillExchangeRatesRequest : ICommandHandler<BackFillExchangeRatesRequest.Command, Unit>
{
    private readonly IBcseApiClient _bcseApiClient;
    private readonly IFinanceDbContext _dbContext;
    private readonly IExchangeRateComposer _composer;

    public BackFillExchangeRatesRequest(
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
        var dates = GetLastDayOfEachMonth(new DateOnly(2020, 1, 1), DateOnly.FromDateTime(DateTime.UtcNow.ToLocal(LocationType.Minsk)));

        var exchangeRatesByDate = await _bcseApiClient.GetExchangeRatesByDates(dates, cancellationToken);

        foreach (var exchangeRateByDate in exchangeRatesByDate)
        {
            if (exchangeRateByDate.ExchangeRates.Count == 0)
            {
                await SetAlternativeExchangeRates(exchangeRateByDate, cancellationToken);
            }

            var existingExchageRates = await _dbContext.ExchangeRates
                .AsNoTracking()
                .Where(x => x.ExchangeDate.Month == exchangeRateByDate.Date.Month &&
                            x.ExchangeDate.Year == exchangeRateByDate.Date.Year)
                .Select(x => new
                {
                    x.FromCurrencyId,
                    x.ToCurrencyId
                })
                .ToListAsync(cancellationToken);

            var newExchangeRates = exchangeRateByDate.ExchangeRates
              .Where(x => !existingExchageRates.Any(y =>
                  y.FromCurrencyId == x.CurrencyCode &&
                  y.ToCurrencyId == CurrencyCodes.BelarusianRuble))
              .ToList();

            if (newExchangeRates.Count == 0)
            {
                continue;
            }

            await _composer.Compose(exchangeRateByDate with { ExchangeRates = newExchangeRates },
                CurrencyCodes.BelarusianRuble, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private async Task SetAlternativeExchangeRates(ExchangeRatesByDateModel exchangeRateByDate, CancellationToken cancellationToken)
    {
        var date = exchangeRateByDate.Date;
        var dateTo = new DateOnly(date.Year, date.Month, 1);

        while (date >= dateTo)
        {
            var result = await _bcseApiClient.GetExchangeRatesByDate(date, cancellationToken);

            if (result.ExchangeRates.Count > 0)
            {
                exchangeRateByDate.ExchangeRates = result.ExchangeRates;

                return;
            }

            date = date.AddDays(-1);
        }
    }

    private static List<DateOnly> GetLastDayOfEachMonth(DateOnly startDate, DateOnly endDate)
    {
        var dates = new List<DateOnly>();
        var current = startDate;

        while (true)
        {
            var lastDayOfMonth = new DateOnly(current.Year, current.Month, 1)
                .AddMonths(1)
                .AddDays(-1);

            if (lastDayOfMonth > endDate)
            {
                dates.Add(endDate);

                break;
            }

            dates.Add(lastDayOfMonth);
            current = current.AddMonths(1);
        }

        return dates.Distinct().ToList();
    }

    public sealed record Command() : ICommand<Unit>, ITransactionalRequest;
}
