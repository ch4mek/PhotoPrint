using System.ComponentModel;

namespace Modsen.Portal.Core.TeamBoard;

public enum InternshipType
{
    [Description("Primary")]
    Primary = 0,
    [Description("Extended")]
    Extended = 1,
    [Description("NDA")]
    NDA = 2,
}
