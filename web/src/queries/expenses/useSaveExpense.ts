import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import {
	EXPENSES_API_ENDPOINT,
	getExpenseByIdEndpoint,
} from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import { MonthExpense, SaveExpenseVariables } from '@/types/expenses';

export const useSaveExpense = (
	options?: UseMutationOptions<
		MonthExpense,
		AxiosError<ServerResponseError>,
		SaveExpenseVariables
	>
) =>
	useMutation({
		mutationFn: async ({
			expenseId,
			previousAmount: _previousAmount,
			...payload
		}) => {
			try {
				if (expenseId) {
					const response = await axios.patch<MonthExpense>(
						getExpenseByIdEndpoint(expenseId),
						payload
					);

					return response.data;
				}

				const response = await axios.post<MonthExpense>(
					EXPENSES_API_ENDPOINT,
					payload
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
