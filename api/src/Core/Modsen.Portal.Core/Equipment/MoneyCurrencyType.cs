using System.ComponentModel;

namespace Modsen.Portal.Core.Equipment;

public enum MoneyCurrencyType
{
    [Description("Unknown")]
    Unknown = 0,

    [Description("BYN")]
    Byn = 1,

    [Description("EUR")]
    Eur = 2,

    [Description("PLN")]
    Pln = 3,

    [Description("USD")]
    Usd = 4
}
