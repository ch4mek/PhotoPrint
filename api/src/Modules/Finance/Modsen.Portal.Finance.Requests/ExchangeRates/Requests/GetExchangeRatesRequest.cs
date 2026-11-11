using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Finance;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.Requests.Core.CoreDto;
using Modsen.Portal.Finance.Requests.ExchangeRates.Requests.RequestDtos;

namespace Modsen.Portal.Finance.Requests.ExchangeRates.Requests;

internal sealed class GetExchangeRatesRequest : IQueryHandler<GetExchangeRatesRequest.Query, List<ExchangeRateDto>>
{
    private readonly IFinanceDbContext _dbContext;

    public GetExchangeRatesRequest(IFinanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ExchangeRateDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var dateFromDb = await _dbContext.ExchangeRates
            .AsNoTracking()
            .Where(x => x.ExchangeDate.Month == request.Date.Month &&
                        x.ExchangeDate.Year == request.Date.Year)
            .Select(x => x.ExchangeDate)
            .FirstOrDefaultAsync(cancellationToken);

        var dateForSearch = request.Date.Day != dateFromDb.Day
            ? dateFromDb
            : request.Date;

        var exchangeRates = await _dbContext.ExchangeRates
            .AsNoTracking()
            .Include(x => x.FromCurrency)
            .Include(x => x.ToCurrency)
            .Where(x => x.ExchangeDate == dateForSearch)
            .Select(x => new ExchangeRateDto
            {
                Id = x.Id,
                FromCurrency = CurrencyCoreDto.Of(x.FromCurrency!),
                ToCurrency = CurrencyCoreDto.Of(x.ToCurrency!),
                CurrencyMultiplier = x.CurrencyMultiplier,
                Rate = x.Rate,
                ExchangeRateDate = x.ExchangeDate,
                OrderWeight = SetOrderWeight(x.FromCurrency!.CurrencyCode)
            })
            .ToListAsync(cancellationToken);

        if (exchangeRates.Count == 0)
        {
            throw new EntityNotFoundException($"There are no exchange rates for year: {request.Date.Year} and month: {request.Date.Month} in db.");
        }

        return exchangeRates
            .OrderBy(x => x.OrderWeight)
                .ThenBy(x => x.FromCurrency!.CurrencyCode)
            .ToList();
    }

    private static int SetOrderWeight(string currencyCode)
    {
        return currencyCode switch
        {
            CurrencyCodes.USDollar => 1,
            CurrencyCodes.Euro => 2,
            CurrencyCodes.RussianRuble => 3,
            CurrencyCodes.PolishZloty => 4,
            _ => 5,
        };
    }

    internal sealed record Query(DateOnly Date) : IQuery<List<ExchangeRateDto>>;
}
