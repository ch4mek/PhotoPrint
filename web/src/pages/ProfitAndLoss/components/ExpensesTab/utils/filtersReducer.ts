import { Dispatch } from 'react';

import { initialExpensesFilters } from '../constants';
import {
	ExpensesFilters,
	ExpensesFiltersAction,
	ExpensesFiltersActionType,
} from '../types';

export const expensesFiltersReducer = (
	state: ExpensesFilters,
	action: ExpensesFiltersAction
): ExpensesFilters => {
	switch (action.type) {
		case ExpensesFiltersActionType.SetExpenseFilter:
			return { ...state, expense: action.payload };

		case ExpensesFiltersActionType.Set:
			return { ...action.payload };

		case ExpensesFiltersActionType.Reset:
			return initialExpensesFilters;

		default:
			return state;
	}
};

export const createExpensesSetFilters = (
	dispatch: Dispatch<ExpensesFiltersAction>
) => ({
	expense: (payload: string) =>
		dispatch({ type: ExpensesFiltersActionType.SetExpenseFilter, payload }),

	set: (payload: ExpensesFilters) =>
		dispatch({ type: ExpensesFiltersActionType.Set, payload }),

	reset: () => dispatch({ type: ExpensesFiltersActionType.Reset }),
});
