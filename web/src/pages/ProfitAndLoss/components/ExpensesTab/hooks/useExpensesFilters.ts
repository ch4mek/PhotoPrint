import { initialExpensesFilters } from '../constants';
import { ExpensesFilters } from '../types';
import { compareExpensesFilters } from '../utils/compareExpensesFilters';
import {
	createExpensesSetFilters,
	expensesFiltersReducer,
} from '../utils/filtersReducer';

import { useFilters } from '@/hooks/useFilters';

export const useExpensesFilters = (initialValues?: ExpensesFilters) =>
	useFilters(
		initialExpensesFilters,
		expensesFiltersReducer,
		createExpensesSetFilters,
		initialValues,
		compareExpensesFilters
	);
