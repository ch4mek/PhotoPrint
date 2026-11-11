import { SortDirection } from '@/constants/sort';
import { ByPageResponse } from '@/queries/types';

export interface ExpenseItemCategory {
	id: number;
	name: string;
	orderWeight: number;
	isDeleted: boolean;
}

export interface MonthExpense {
	id: number;
	monthOfYear: number;
	expenseAmount: number;
}

export interface ExpenseItem {
	category: ExpenseItemCategory;
	expensesByMonth: MonthExpense[];
}

export type ExpensesResponse = {
	categoriesExpenses: ByPageResponse<ExpenseItem>;
	expensesSumsByMonth: number[];
};

interface SaveExpensePayload {
	expenseCategoryId: number;
	year: number;
	monthOfYear: number;
	expenseAmount: number;
}

export interface SaveExpenseVariables extends SaveExpensePayload {
	expenseId?: number;
	previousAmount?: number;
}

export interface GetExpensesRequestBody {
	orderingField: string | null;
	isDescendingOrdering: boolean;
	categoryName: string | null;
}

export interface GetExpensesQueryKeyParams {
	page: number;
	top: number;
	order: SortDirection;
	orderBy: string;
	year: string | number;
	expense?: string;
}

export type ExpenseCategoriesLookupResponse = string[];

export interface ExpenseCategoryPayload {
	categoryName: string;
}

export interface UpdateExpenseCategoryVariables extends ExpenseCategoryPayload {
	id: number;
}
