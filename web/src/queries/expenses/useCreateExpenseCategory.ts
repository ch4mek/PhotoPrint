import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { EXPENSE_CATEGORIES_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import { ExpenseCategoryPayload, ExpenseItemCategory } from '@/types/expenses';

export const useCreateExpenseCategory = (
	options?: UseMutationOptions<
		ExpenseItemCategory,
		AxiosError<ServerResponseError>,
		ExpenseCategoryPayload
	>
) =>
	useMutation({
		mutationFn: async (payload) => {
			try {
				const response = await axios.post<ExpenseItemCategory>(
					EXPENSE_CATEGORIES_API_ENDPOINT,
					payload
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
