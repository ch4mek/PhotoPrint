import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { expensesQueriesKeys } from '@/constants/queriesKeys';
import { GET_EXPENSE_CATEGORIES_LOOKUP_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ExpenseCategoriesLookupResponse } from '@/types/expenses';

export const useExpenseCategoriesLookup = () =>
	useQueryWithCallback<ExpenseCategoriesLookupResponse, AxiosError>({
		queryKey: [expensesQueriesKeys.getExpenseCategoriesLookup],
		queryFn: () =>
			axios
				.get<ExpenseCategoriesLookupResponse>(
					GET_EXPENSE_CATEGORIES_LOOKUP_ENDPOINT
				)
				.then((res) => res.data),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});
