using System.ComponentModel;

namespace Modsen.Portal.Core.Inquiries;

public enum DateType
{
    [Description("Holiday")]
    Holiday,
    [Description("DayOff")]
    DayOff,
    [Description("WorkingDay")]
    WorkingDay,
    [Description("Saturday")]
    Saturday,
    [Description("Sunday")]
    Sunday
}
