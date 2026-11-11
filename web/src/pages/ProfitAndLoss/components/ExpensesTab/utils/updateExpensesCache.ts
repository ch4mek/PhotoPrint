import { QueryClient } from '@tanstack/react-query';

import { expensesQueriesKeys } from '@/constants/queriesKeys';
import { getExpensesQueryYear } from '@/queries/expenses/utils/getExpensesQueryKey';
import {
	ExpensesResponse,
	MonthExpense,
	SaveExpenseVariables,
} from '@/types/expenses';

const upsertMonthExpense = (
	expenses: MonthExpense[],
	savedExpense: MonthExpense
) => {
	const index = expenses.findIndex(
		({ monthOfYear }) => monthOfYear === savedExpense.monthOfYear
	);

	if (index === -1) {
		return [...expenses, savedExpense];
	}

	return expenses.map((expense, expenseIndex) =>
		expenseIndex === index ? savedExpense : expense
	);
};

export const updateExpensesCache = (
	queryClient: QueryClient,
	{
		expenseCategoryId,
		year,
		monthOfYear,
		previousAmount = 0,
	}: SaveExpenseVariables,
	savedExpense: MonthExpense
) => {
	const monthIndex = monthOfYear - 1;
	const delta = savedExpense.expenseAmount - previousAmount;

	queryClient
		.getQueriesData<ExpensesResponse>({
			queryKey: [expensesQueriesKeys.getExpenses],
		})
		.forEach(([queryKey, data]) => {
			if (!data || String(getExpensesQueryYear(queryKey)) !== String(year)) {
				return;
			}

			queryClient.setQueryData<ExpensesResponse>(queryKey, {
				...data,
				expensesSumsByMonth: data.expensesSumsByMonth.map((sum, index) =>
					index === monthIndex ? sum + delta : sum
				),
				categoriesExpenses: {
					...data.categoriesExpenses,
					items: data.categoriesExpenses.items.map((item) =>
						item.category.id === expenseCategoryId
							? {
									...item,
									expensesByMonth: upsertMonthExpense(
										item.expensesByMonth,
										savedExpense
									),
							  }
							: item
					),
				},
			});
		});
};
