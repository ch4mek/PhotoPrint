
namespace Modsen.Portal.Finance.Requests.Currencies.Core.CoreDto;

public class CurrencyLookupDto
{
    public required string Id { get; set; }
    public string? Name { get; set; }
    public required string CurrencyCode { get; set; }
    public string? CurrencySymbol { get; set; }
}
