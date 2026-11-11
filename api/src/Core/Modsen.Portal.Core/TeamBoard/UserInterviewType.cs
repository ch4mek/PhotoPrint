using System.ComponentModel;

namespace Modsen.Portal.Core.TeamBoard;

public enum UserInterviewType
{
    [Description("Prescreen")]
    Prescreen = 0,

    [Description("Tech")]
    Tech = 1,

    [Description("Final")]
    Final = 2,

    [Description("Other")]
    Other = 3
}
