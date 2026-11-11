namespace Modsen.Portal.TeamBoard.Requests.Home.Requests.RequestsDto;

internal class DayWithBirthdaysDto
{
    public int Day { get; set; }
    public List<UserBirthdayDto> Users { get; set; } = [];
}
