namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Output;

public class WorkingHoursTotalOutput
{
    public required int CountryId { get; set; }
    public required int Year { get; set; }
    public required int Hours { get; set; }
}
