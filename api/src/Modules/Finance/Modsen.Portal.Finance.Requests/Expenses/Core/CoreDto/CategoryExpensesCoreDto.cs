using Modsen.Portal.Finance.Requests.Core.CoreDto;

namespace Modsen.Portal.Finance.Requests.Expenses.Core.CoreDto;

public class CategoryExpensesCoreDto
{
    public required ExpenseCategoryCoreDto Category {  get; set; }
    public IReadOnlyList<ExpenseShortCoreDto?> ExpensesByMonth { get; set; } = new List<ExpenseShortCoreDto?>(12);
}
