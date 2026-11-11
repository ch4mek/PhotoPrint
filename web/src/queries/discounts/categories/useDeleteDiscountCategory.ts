import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { getDiscountCategoryByIdEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import { DiscountCategory } from '@/types/discountCategories';

export const useDeleteDiscountCategory = (
	options?: UseMutationOptions<
		DiscountCategory,
		AxiosError<ServerResponseError>,
		number | string
	>
) =>
	useMutation({
		mutationFn: async (id) => {
			try {
				const response = await axios.delete(
					getDiscountCategoryByIdEndpoint(id)
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
