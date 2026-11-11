import { useMemo } from 'react';

import { ExpensesFilterOptions } from '../types';

import { useExpenseCategoriesLookup } from '@/queries/expenses/useExpenseCategoriesLookup';

export const useExpensesFilterOptions = () => {
	const { data: expenseCategories } = useExpenseCategoriesLookup();

	const options = useMemo(
		(): ExpensesFilterOptions => ({
			expenses: expenseCategories ?? [],
		}),
		[expenseCategories]
	);

	return { options };
};
