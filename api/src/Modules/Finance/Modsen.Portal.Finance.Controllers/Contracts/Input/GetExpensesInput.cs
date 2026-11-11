using Modsen.Portal.Finance.Requests.Expenses.Core.CoreDto;

namespace Modsen.Portal.Finance.Controllers.Contracts.Input;

public sealed class GetExpensesInput
{
    public string? CategoryName { get; set; }

    public ExpenseOrderingField? OrderingField { get; set; }
    public bool? IsDescendingOrdering { get; set; }
}
