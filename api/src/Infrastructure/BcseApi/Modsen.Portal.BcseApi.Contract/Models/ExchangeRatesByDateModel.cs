namespace Modsen.Portal.BcseApi.Contract.Models;

public sealed record ExchangeRatesByDateModel
{
    public required DateOnly Date { get; init; }
    public IReadOnlyCollection<ExchangeRateModel> ExchangeRates { get; set; } = [];
}
