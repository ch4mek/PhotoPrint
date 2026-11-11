namespace Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;

internal class FinanceOrderingCoreDto
{
    public FinanceOrderingField Field { get; set; }
    public bool IsDescending { get; set; }
}
public enum FinanceOrderingField
{
    UserId = 0,
    UserFullName = 1,
    DepartmentId = 2
}
