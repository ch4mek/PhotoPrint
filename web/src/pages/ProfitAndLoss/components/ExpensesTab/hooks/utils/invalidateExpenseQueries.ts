import { QueryClient } from '@tanstack/react-query';

import { expensesQueriesKeys } from '@/constants/queriesKeys';

export const invalidateExpenseQueries = (queryClient: QueryClient) => {
	queryClient.invalidateQueries({
		queryKey: [expensesQueriesKeys.getExpenses],
	});
	queryClient.invalidateQueries({
		queryKey: [expensesQueriesKeys.getExpenseCategoriesLookup],
	});
};
