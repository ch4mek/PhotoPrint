using Modsen.Portal.Finance.Requests.ExchangeRates.Requests.RequestDtos;

namespace Modsen.Portal.Finance.Controllers.Contracts.Output;

internal sealed class ExchangeRateOutput
{
    public int Id { get; init; }
    public required CurrencyOutput FromCurrency { get; init; }
    public required CurrencyOutput ToCurrency { get; init; }
    public required int CurrencyMultiplier { get; init; }
    public required decimal Rate { get; init; }

    public static ExchangeRateOutput Of(ExchangeRateDto source)
    {
        return new ExchangeRateOutput
        {
            Id = source.Id,
            FromCurrency = CurrencyOutput.Of(source.FromCurrency),
            ToCurrency = CurrencyOutput.Of(source.ToCurrency),
            CurrencyMultiplier = source.CurrencyMultiplier,
            Rate = source.Rate
        };
    }
}
