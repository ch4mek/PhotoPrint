using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Finance.Requests.Expenses.Core.CoreDto;

internal class ExpensesQueryBuilderDto
{
    public List<ExpenseCategoryEntity> Categories { get; set; } = [];
    public int TotalCount { get; set; }
}
