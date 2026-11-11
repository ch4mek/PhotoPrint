using Modsen.Portal.Finance.Requests.Core.CoreDto;

namespace Modsen.Portal.Finance.Controllers.Contracts.Output;

public sealed class CurrencyOutput
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string CurrencyCode { get; init; }
    public string? CurrencySymbol { get; init; }

    internal static CurrencyOutput Of(CurrencyCoreDto currency)
    {
        return new CurrencyOutput
        {
            Id = currency.Id,
            Name = currency.Name,
            CurrencyCode = currency.CurrencyCode,
            CurrencySymbol = currency.CurrencySymbol
        };
    }
}
