import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { getExpenseCategoryByIdEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import {
	ExpenseItemCategory,
	UpdateExpenseCategoryVariables,
} from '@/types/expenses';

export const useUpdateExpenseCategory = (
	options?: UseMutationOptions<
		ExpenseItemCategory,
		AxiosError<ServerResponseError>,
		UpdateExpenseCategoryVariables
	>
) =>
	useMutation({
		mutationFn: async ({ id, ...payload }) => {
			try {
				const response = await axios.patch<ExpenseItemCategory>(
					getExpenseCategoryByIdEndpoint(id),
					payload
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
