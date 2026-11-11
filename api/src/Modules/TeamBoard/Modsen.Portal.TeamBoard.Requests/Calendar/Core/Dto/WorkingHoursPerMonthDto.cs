namespace Modsen.Portal.TeamBoard.Requests.Calendar.Core.Dto;

internal class WorkingHoursPerMonthDto
{
    public int Id { get; set; }
    public int Year { get; set; }
    public int MonthOfYear { get; set; }
    public int CountryId { get; set; }
    public int Hours { get; set; }
}
