namespace Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;

internal class WorkingMonthDto
{
    public IReadOnlyCollection<DayOfMonthDto> Holidays { get; set; } = [];
    public IReadOnlyCollection<DayOfMonthDto> Leaves { get; set; } = [];
    public IReadOnlyCollection<DayOfMonthDto> Weekends { get; set; } = [];
    public IReadOnlyCollection<DayOfMonthDto> DaysOfMonth { get; set; } = [];
    public IReadOnlyCollection<DayOfMonthDto> WorkingDays { get; set; } = [];
}
