using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Finance.Requests.Core.CoreDto;

internal class CurrencyCoreDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string CurrencyCode { get; init; }
    public string? CurrencySymbol { get; init; }

    public static CurrencyCoreDto Of(CurrencyEntity currency)
    {
        return new CurrencyCoreDto
        {
            Id = currency.Id,
            Name = currency.Name,
            CurrencyCode = currency.CurrencyCode,
            CurrencySymbol = currency.CurrencySymbol
        };
    }
}
