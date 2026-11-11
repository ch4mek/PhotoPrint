namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

internal class CalendarWorkingHourDetailsCoreDto
{
    public int MonthOfYear { get; set; }
    public List<CalendarCountryHourDetailsCoreDto> CountryHours { get; set; } = [];
}
