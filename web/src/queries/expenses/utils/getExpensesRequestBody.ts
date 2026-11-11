import { SortDirection } from '@/constants/sort';
import { EXPENSES_ORDERING_FIELD } from '@/pages/ProfitAndLoss/components/ExpensesTab/constants';
import { ExpensesFilters } from '@/pages/ProfitAndLoss/components/ExpensesTab/types';
import { GetExpensesRequestBody } from '@/types/expenses';

export const getExpensesRequestBody = (
	order: SortDirection,
	{ expense }: ExpensesFilters
): GetExpensesRequestBody => {
	const categoryName = expense.trim() || null;

	return {
		orderingField: EXPENSES_ORDERING_FIELD,
		isDescendingOrdering: order === SortDirection.DESC,
		categoryName,
	};
};
