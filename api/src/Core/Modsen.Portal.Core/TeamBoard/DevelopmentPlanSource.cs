using System.ComponentModel;

namespace Modsen.Portal.Core.TeamBoard;

public enum DevelopmentPlanSource
{
    Unknown = 0,
    Popup = 1
}

public enum DevelopmentPlanStatusType
{
    [Description("None")]
    None = 0,

    [Description("Rejected")]
    Rejected = 1,

    [Description("Under Review")]
    UnderReview = 2,

    [Description("Approved")]
    Approved = 3,
}
