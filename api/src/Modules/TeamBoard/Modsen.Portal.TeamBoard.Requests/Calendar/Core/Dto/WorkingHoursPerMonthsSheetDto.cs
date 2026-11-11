namespace Modsen.Portal.TeamBoard.Requests.Calendar.Core.Dto;

internal class WorkingHoursPerMonthsSheetDto
{
    public IReadOnlyCollection<WorkingHoursPerMonthDto> HoursPerMonth { get; set; } = [];
    public IReadOnlyCollection<WorkingHoursTotalDto> HoursPerYear { get; set; } = [];
}
