import { initialExpensesFilters } from '../constants';
import { ExpensesFilters } from '../types';

export const compareExpensesFilters = ({ expense }: ExpensesFilters) => ({
	expense: expense === initialExpensesFilters.expense,
});
