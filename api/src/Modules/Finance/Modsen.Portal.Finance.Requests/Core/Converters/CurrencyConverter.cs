using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Finance.DataAccess.Npgsql;

namespace Modsen.Portal.Finance.Requests.Core.Converters;

internal interface ICurrencyConverter
{
    Task<CurrencyConverter.Response> Convert(CurrencyConverter.Request request, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<CurrencyConverter.Response>> Convert(IReadOnlyCollection<CurrencyConverter.Request> requests, CancellationToken cancellationToken);
}

internal class CurrencyConverter : ICurrencyConverter
{
    private readonly IFinanceDbContext _financeDbContext;

    public CurrencyConverter(IFinanceDbContext financeDbContext)
    {
        _financeDbContext = financeDbContext;
    }

    public async Task<Response> Convert(Request request, CancellationToken cancellationToken)
    {
        var response = Response.Of(request);

        var exchangeRate = await _financeDbContext.ExchangeRates
            .AsNoTracking()
            .Where(x => x.FromCurrencyId == request.FromCurrencyCode && x.ToCurrencyId == request.ToCurrencyCode)
            .Where(x => x.ExchangeDate.Year == request.Year && x.ExchangeDate.Month == request.Month)
            .Select(x => new
            {
                x.CurrencyMultiplier,
                x.Rate
            })
            .FirstOrDefaultAsync(cancellationToken);

        response.Result = exchangeRate != null
            ? Calculate(request.FromCurrencyCount, exchangeRate.CurrencyMultiplier, exchangeRate.Rate)
            : throw new AppException($"Error during currency conversion. There is no {request.FromCurrencyCode} to {request.ToCurrencyCode} exchange rate.");

        return response;
    }

    public async Task<IReadOnlyCollection<Response>> Convert(IReadOnlyCollection<Request> requests, CancellationToken cancellationToken)
    {
        if (requests.Count == 0)
        {
            return [];
        }

        var fromCurrencyCodes = requests.Select(y => y.FromCurrencyCode).Distinct().ToHashSet();
        var toCurrencyCode = requests.Select(y => y.ToCurrencyCode).Distinct().ToHashSet();
        var years = requests.Select(y => y.Year).Distinct().ToHashSet();
        var months = requests.Select(y => y.Month).Distinct().ToHashSet();

        var exchangeRates = await _financeDbContext.ExchangeRates
            .AsNoTracking()
            .Where(x => fromCurrencyCodes.Contains(x.FromCurrencyId) &&
                        toCurrencyCode.Contains(x.ToCurrencyId) &&
                        years.Contains(x.ExchangeDate.Year) &&
                        months.Contains(x.ExchangeDate.Month))
            .ToListAsync(cancellationToken);

        if (exchangeRates.Count == 0)
        {
            throw new AppException("Error during currency conversion. The required exchange rates do not exist.");
        }

        var response = new List<Response>();

        foreach (var request in requests)
        {
            var currencyConverterResponse = Response.Of(request);

            var exchangeRate = exchangeRates
                .FirstOrDefault(x => x.ExchangeDate.Year == request.Year &&
                            x.ExchangeDate.Month == request.Month &&
                            x.FromCurrencyId == request.FromCurrencyCode &&
                            x.ToCurrencyId == request.ToCurrencyCode);

            if (exchangeRate == null)
            {
                throw new AppException($"Error during currency conversion. There is no {request.FromCurrencyCode} to {request.ToCurrencyCode} exchange rate.");
            }

            currencyConverterResponse.Result = Calculate(request.FromCurrencyCount, exchangeRate.CurrencyMultiplier, exchangeRate.Rate);

            response.Add(currencyConverterResponse);
        }

        return response;
    }

    private static decimal Calculate(decimal count, int currencyMultiplier, decimal rate)
    {
        return rate / (decimal)currencyMultiplier * count;
    }

    internal sealed class Request
    {
        public required string FromCurrencyCode { get; init; }
        public decimal FromCurrencyCount { get; init; }
        public required string ToCurrencyCode { get; init; }
        public int Year { get; init; }
        public int Month { get; init; }
    }

    internal sealed class Response
    {
        public required string FromCurrencyCode { get; init; }
        public decimal FromCurrencyCount { get; init; }
        public required string ToCurrencyCode { get; init; }
        public int Year { get; init; }
        public int Month { get; init; }

        public decimal Result { get; set; }

        public static Response Of(Request source)
        {
            return new Response
            {
                FromCurrencyCode = source.FromCurrencyCode,
                FromCurrencyCount = source.FromCurrencyCount,
                ToCurrencyCode = source.ToCurrencyCode,
                Year = source.Year,
                Month = source.Month
            };
        }
    }
}
