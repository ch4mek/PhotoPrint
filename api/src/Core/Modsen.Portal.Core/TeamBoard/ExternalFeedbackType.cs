using System.ComponentModel;

namespace Modsen.Portal.Core.TeamBoard;

public enum ExternalFeedbackType
{
    [Description("Unknown")]
    Unknown = 0,

    [Description("Bad")]
    Bad = 1,

    [Description("Normal")]
    Normal = 2,

    [Description("Great")]
    Great = 3
}
