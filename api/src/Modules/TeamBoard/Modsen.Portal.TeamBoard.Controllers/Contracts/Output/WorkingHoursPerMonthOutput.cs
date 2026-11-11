namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Output;

public class WorkingHoursPerMonthOutput
{
    public required int Id { get; set; }
    public required int Year { get; set; }
    public required int MonthOfYear { get; set; }
    public required int CountryId { get; set; }
    public required int Hours { get; set; }
}
