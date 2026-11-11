using System.ComponentModel;

namespace Modsen.Portal.Core.Identity
{
    public enum MessageRole
    {
        [Description("User")]
        User = 0,
        [Description("Assistant")]
        Assistant = 1,
    }

    public enum MessageSource
    {
        [Description("Teams")]
        Teams = 0,
        [Description("MMS")]
        MMS = 1,
    }
}
