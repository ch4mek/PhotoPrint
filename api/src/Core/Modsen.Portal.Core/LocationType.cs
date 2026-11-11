using System.ComponentModel;

namespace Modsen.Portal.Core;

public enum LocationType
{
    [Description("Unknown")]
    Unknown = 0,

    [Description("Minsk")]
    Minsk = 1,

    [Description("Vitebsk")]
    Vitebsk = 2,

    [Description("Warsaw")]
    Warsaw = 3,

    [Description("Tbilisi")]
    Tbilisi = 4
}
