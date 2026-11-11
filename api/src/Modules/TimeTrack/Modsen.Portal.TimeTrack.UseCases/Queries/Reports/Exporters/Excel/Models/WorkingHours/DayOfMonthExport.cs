using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;

namespace Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Excel.Models.WorkingHours;

public class DayOfMonthToExcelExportModel
{
    public DateOnly CurrentDate { get; set; }
    public int? Hours { get; set; }
    public MonthsDayType MonthsDayType { get; set; }

    internal static DayOfMonthToExcelExportModel Of(DayOfMonthDto source)
    {
        return new DayOfMonthToExcelExportModel
        {
            CurrentDate = source.CurrentDate,
            Hours = source.Hours,
            MonthsDayType = source.MonthsDayType,
        };
    }
}
