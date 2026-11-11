import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_DISCOUNT_CATEGORIES_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import {
	DiscountCategory,
	DiscountCategoryMutationData,
} from '@/types/discountCategories';

export const useCreateDiscountCategory = (
	options?: UseMutationOptions<
		DiscountCategory,
		AxiosError<ServerResponseError>,
		DiscountCategoryMutationData
	>
) =>
	useMutation({
		mutationFn: async (data) => {
			try {
				const response = await axios.post(
					GET_DISCOUNT_CATEGORIES_API_ENDPOINT,
					data
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
