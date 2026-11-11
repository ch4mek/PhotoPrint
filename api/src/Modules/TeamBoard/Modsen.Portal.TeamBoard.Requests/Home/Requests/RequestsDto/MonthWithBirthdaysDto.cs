namespace Modsen.Portal.TeamBoard.Requests.Home.Requests.RequestsDto;

internal class MonthWithBirthdaysDto
{
    public int Month { get; set; }
    public int BirthdaysCount => Days.Sum(day => day.Users.Count);
    public List<DayWithBirthdaysDto> Days { get; set; } = [];
}
