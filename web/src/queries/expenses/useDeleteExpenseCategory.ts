import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { getExpenseCategoryByIdEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import { ExpenseItemCategory } from '@/types/expenses';

export const useDeleteExpenseCategory = (
	options?: UseMutationOptions<
		ExpenseItemCategory,
		AxiosError<ServerResponseError>,
		number
	>
) =>
	useMutation({
		mutationFn: async (id) => {
			try {
				const response = await axios.delete<ExpenseItemCategory>(
					getExpenseCategoryByIdEndpoint(id)
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
