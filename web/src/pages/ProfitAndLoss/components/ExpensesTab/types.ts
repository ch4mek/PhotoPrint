import { MonthExpense } from '@/types/expenses';
import { HandleRowClick } from '@/types/grid';
import { SetFilters } from '@/types/helpers';

export interface ExpensesGridRow {
	id: number;
	expense: string;
	expensesByMonth: MonthExpense[];
}

export interface ExpensesSummaryGridRow {
	values: string[];
}

export interface ExpensesFilters {
	expense: string;
}

export interface ExpensesFilterOptions {
	expenses: string[];
}

export interface ExpensesSetFilters extends SetFilters<ExpensesFilters> {}

export enum ExpensesFiltersActionType {
	SetExpenseFilter = 'SetExpenseFilter',
	Reset = 'Reset',
	Set = 'Set',
}

export type ExpensesFiltersAction =
	| { type: ExpensesFiltersActionType.SetExpenseFilter; payload: string }
	| { type: ExpensesFiltersActionType.Set; payload: ExpensesFilters }
	| { type: ExpensesFiltersActionType.Reset };

export interface UseExpensesColumnsProps {
	filters: ExpensesFilters;
	setFilters: ExpensesSetFilters;
	isResetFiltersDisabled: boolean;
	monthColumnWidth: number;
	handleEdit: HandleRowClick<ExpensesGridRow>;
	handleDelete: HandleRowClick<ExpensesGridRow>;
	filterOptions: ExpensesFilterOptions;
}
