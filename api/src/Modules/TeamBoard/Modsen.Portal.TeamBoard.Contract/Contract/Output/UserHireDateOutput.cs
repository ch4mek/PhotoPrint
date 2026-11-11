namespace Modsen.Portal.TeamBoard.Contract.Contract.Output;

public class UserHireDateOutput
{
    public required int UserId { get; set; }
    public required DateOnly HireDate {  get; set; }
}
