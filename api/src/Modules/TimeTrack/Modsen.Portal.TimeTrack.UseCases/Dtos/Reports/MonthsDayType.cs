using System.ComponentModel;

namespace Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;

public enum MonthsDayType
{
    [Description("-")]
    Unknown = 0,

    [Description("О")]
    Vacation = 1,

    [Description("П")]
    Holiday = 2,

    [Description("В")]
    Weekend = 3,

    [Description("А")]
    OwnExpenseVacation = 4,

    [Description("Б")]
    SickLeave = 5,

    [Description("Р")]
    WorkingDay = 6,

    [Description("-")]
    None = 7
}
