using System.ComponentModel;

namespace Modsen.Portal.Core.Merch;

public enum StorageAvailabilityStatus
{
    [Description("Unknown")]
    Unknown = 0,

    [Description("Running out")]
    RunningOut = 1,

    [Description("Available")]
    Available = 2,

    [Description("Archived")]
    Archived = 3
}
