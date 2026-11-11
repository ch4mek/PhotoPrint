namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

public class CreateWorkingHoursPerMonthInput
{
    public required int Year { get; set; }
    public required int MonthOfYear { get; set; }
    public required int CountryId { get; set; }
    public required int Hours { get; set; }
}
