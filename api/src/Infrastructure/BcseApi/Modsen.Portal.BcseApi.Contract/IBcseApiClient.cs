using Modsen.Portal.BcseApi.Contract.Models;

namespace Modsen.Portal.BcseApi.Contract;

public interface IBcseApiClient
{
    Task<ExchangeRatesByDateModel> GetCurrentExchangeRates(CancellationToken cancellationToken);
    Task<ExchangeRatesByDateModel> GetExchangeRatesByDate(DateOnly targetDate, CancellationToken cancellationToken);
    Task<List<ExchangeRatesByDateModel>> GetExchangeRatesByDates(IReadOnlyCollection<DateOnly> dates, CancellationToken cancellationToken);
}
