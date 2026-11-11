namespace Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;

internal class DayOfMonthDto
{
    public int? EmployeeId { get; set; }
    public DateOnly CurrentDate { get; set; }
    public int? Hours { get; set; }
    public MonthsDayType MonthsDayType { get; set; }
}
