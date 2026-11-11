namespace Modsen.Portal.BcseApi.Contract.Models;

public class ExchangeRateModel
{
    public required string CurrencyName { get; init; }
    public required string CurrencyCode { get; init; }
    public int RateCoef { get; init; }
    public decimal RateNB { get; init; }
}
