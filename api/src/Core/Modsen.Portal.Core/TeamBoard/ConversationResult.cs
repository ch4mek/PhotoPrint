using System.ComponentModel;

namespace Modsen.Portal.Core.TeamBoard;

public enum ConversationResult
{
    [Description("Bad")]
    Bad = 0,

    [Description("Normal")]
    Normal = 1,

    [Description("Great")]
    Great = 2
}
