namespace Modsen.Portal.Finance.Requests.Expenses.Core.CoreDto;

internal class ExpenseOrderingCoreDto
{
    public ExpenseOrderingField Field { get; set; }
    public bool IsDescending { get; set; }
}

public enum ExpenseOrderingField
{
    OrderWeight = 0,
    CategoryName = 1,
    MonthExpensesAmount = 2,
}
