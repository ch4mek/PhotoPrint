using Modsen.Portal.Finance.Requests.ExchangeRates.Requests.RequestDtos;

namespace Modsen.Portal.Finance.Controllers.Contracts.Output;

internal sealed class ExchangeRatesByDateOutput
{
    public DateOnly? ExchangeRateDate { get; init; }
    public List<ExchangeRateOutput> ExchangeRates { get; init; } = [];

    public static ExchangeRatesByDateOutput Of(List<ExchangeRateDto> source)
    {
        return new ExchangeRatesByDateOutput
        {
            ExchangeRateDate = source.FirstOrDefault()?.ExchangeRateDate,
            ExchangeRates = source.Select(ExchangeRateOutput.Of).ToList()
        };
    }
}
