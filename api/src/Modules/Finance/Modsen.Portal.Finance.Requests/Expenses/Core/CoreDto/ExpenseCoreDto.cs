using Modsen.Portal.Finance.Requests.Core.CoreDto;

namespace Modsen.Portal.Finance.Requests.Expenses.Core.CoreDto;

public class ExpenseCoreDto
{
    public int Id { get; set; }
    public int ExpenseCategoryId { get; set; }
    public ExpenseCategoryCoreDto? ExpenseCategory { get; set; }
    public int Year { get; set; }
    public int MonthOfYear { get; set; }
    public decimal ExpenseAmount { get; set; }
}
