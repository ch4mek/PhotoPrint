import { expensesQueriesKeys } from '@/constants/queriesKeys';
import { GetExpensesQueryKeyParams } from '@/types/expenses';

export const getExpensesQueryKey = ({
	page,
	top,
	order,
	orderBy,
	year,
	expense = '',
}: GetExpensesQueryKeyParams) =>
	[
		expensesQueriesKeys.getExpenses,
		year,
		page,
		top,
		order,
		orderBy,
		expense,
	] as const;

export const getExpensesQueryYear = (queryKey: readonly unknown[]) =>
	queryKey[1];
