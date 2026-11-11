namespace Modsen.Portal.Finance.Controllers.Contracts.Input;

public record CreateExpenseInput(int ExpenseCategoryId, int Year, int MonthOfYear, decimal ExpenseAmount);
