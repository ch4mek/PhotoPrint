namespace Modsen.Portal.TeamBoard.Contract.Contract.Input;

public class UsersSalaryByMonthInput
{
    public IReadOnlyCollection<int> UserIds { get; init; } = [];
    public int Month { get; init; }
    public int Year { get; init; }
}
