using System.ComponentModel;

namespace Modsen.Portal.Core.TeamBoard;

public enum ConversationType
{
    [Description("Unknown")]
    Unknown = 0,

    [Description("Conversation")]
    Conversation = 1,

    [Description("Performance review")]
    PerformanceReview = 2,

    [Description("HR Conversation")]
    HRConversation = 3
}
