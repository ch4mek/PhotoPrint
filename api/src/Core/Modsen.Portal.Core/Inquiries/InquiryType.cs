using System.ComponentModel;

namespace Modsen.Portal.Core.Inquiries;

public enum InquiryType
{
    [Description("Unknown")]
    Unknown = 0,
    [Description("Vacation")]
    Vacation = 1,
    [Description("Sick day")]
    SickDay = 2,
    [Description("Sick leave")]
    SickLeave = 3,
    [Description("Own expense")]
    OwnExpense = 4,
    [Description("Home office")]
    HomeOffice = 5,
    [Description("Weekend transfer")]
    WeekendTransfer = 6,
    [Description("Military enlistment office")]
    MilitaryEnlistmentOffice = 7,
    [Description("Social vacation")]
    SocialVacation = 8,
    [Description("Documents")]
    Documents = 9,
    [Description("Other type")]
    OtherType = 10
}

public enum TrackingUnit
{
    Days = 0,
    Hours = 1,
}


public enum ActivityType
{
    NonWorkingPaid = 0,
    WorkingPaid = 1,
    NonWorkingUnpaid = 2,
}
