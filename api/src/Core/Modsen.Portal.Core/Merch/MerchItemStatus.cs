using System.ComponentModel;

namespace Modsen.Portal.Core.Merch;

public enum MerchItemStatus
{
    [Description("Available")]
    Available = 0,

    [Description("Archived")]
    Archived = 1,
}
