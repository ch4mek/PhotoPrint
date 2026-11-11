using System.ComponentModel;

namespace Modsen.Portal.Core.TeamBoard;

public enum MilitaryServiceStatus
{
    [Description("Fit")]
    Fit = 0,
    [Description("Unfit")]
    Unfit = 1,
    [Description("Served")]
    Served = 2,
}
