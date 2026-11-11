import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { getExpensesApiEndpoint } from '@/constants/queryPaths';
import { SortDirection } from '@/constants/sort';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ExpensesFilters } from '@/pages/ProfitAndLoss/components/ExpensesTab/types';
import { ServerResponseError } from '@/queries/types';
import { ExpensesResponse } from '@/types/expenses';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

import { getExpensesQueryKey } from './utils/getExpensesQueryKey';
import { getExpensesRequestBody } from './utils/getExpensesRequestBody';

export const useGetExpenses = (
	top: number,
	page: number,
	order: SortDirection,
	orderBy: string,
	year: string,
	filters: ExpensesFilters,
	options?: PatchedQueryOptions<
		ExpensesResponse,
		AxiosError<ServerResponseError>
	>
) => {
	const { expense } = filters;

	return useQueryWithCallback<
		ExpensesResponse,
		AxiosError<ServerResponseError>
	>({
		queryKey: getExpensesQueryKey({
			page,
			top,
			order,
			orderBy,
			year,
			expense,
		}),

		queryFn: async () => {
			try {
				const { data } = await axios.post<ExpensesResponse>(
					getExpensesApiEndpoint(year, top, page),
					getExpensesRequestBody(order, filters)
				);

				return data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		refetchOnMount: true,
		...options,
	});
};
