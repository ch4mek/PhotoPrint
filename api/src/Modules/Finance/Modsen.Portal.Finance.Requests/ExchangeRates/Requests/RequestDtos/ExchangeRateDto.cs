using Modsen.Portal.Finance.Requests.Core.CoreDto;

namespace Modsen.Portal.Finance.Requests.ExchangeRates.Requests.RequestDtos;

internal class ExchangeRateDto
{
    public int Id { get; init; }
    public required CurrencyCoreDto FromCurrency { get; init; }
    public required CurrencyCoreDto ToCurrency { get; init; }
    public required int CurrencyMultiplier { get; init; }
    public required decimal Rate { get; init; }
    public required DateOnly ExchangeRateDate { get; init; }
    public int OrderWeight { get; set; }
}
