namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

public class PayloadOrderingCoreDto
{
    public PayloadOrderingField Field { get; set; }
    public bool IsDescending { get; set; }
}

public enum PayloadOrderingField
{
    UserId = 0,
    EmployeeFullName = 1
}
