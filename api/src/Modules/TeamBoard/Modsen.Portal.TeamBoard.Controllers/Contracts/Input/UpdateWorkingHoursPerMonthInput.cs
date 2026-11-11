namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

public class UpdateWorkingHoursPerMonthInput
{
    public required int Id { get; set; }
    public required int Hours { get; set; }
}
