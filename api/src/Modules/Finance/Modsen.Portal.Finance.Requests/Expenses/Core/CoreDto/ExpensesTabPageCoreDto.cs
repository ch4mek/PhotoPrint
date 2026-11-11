using Modsen.Portal.Core.Utils;

namespace Modsen.Portal.Finance.Requests.Expenses.Core.CoreDto;

public class ExpensesTabPageCoreDto
{
    public required PageableListOfItems<CategoryExpensesCoreDto> CategoriesExpenses { get; set; }
    public IReadOnlyList<decimal> ExpensesSumsByMonth { get; set; } = new decimal[12];
}
