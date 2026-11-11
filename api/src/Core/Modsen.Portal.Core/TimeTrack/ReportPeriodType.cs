using System.ComponentModel;

namespace Modsen.Portal.Core.TimeTrack;

public enum ReportPeriodType
{
    [Description("Unknown")]
    Unknown = 0,

    [Description("annual summary report")]
    Annual = 1,

    [Description("monthly summary report")]
    Monthly = 2,

    [Description("quarter summary report")]
    Quarter = 3,

    [Description("summary report")]
    Arbitrary = 4
}
