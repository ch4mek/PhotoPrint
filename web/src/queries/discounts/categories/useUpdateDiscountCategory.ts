import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { getDiscountCategoryByIdEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import {
	CreateDiscountCategoryMutationData,
	DiscountCategory,
} from '@/types/discountCategories';

export const useUpdateDiscountCategory = (
	options?: UseMutationOptions<
		DiscountCategory,
		AxiosError<ServerResponseError>,
		CreateDiscountCategoryMutationData
	>
) =>
	useMutation({
		mutationFn: async ({ id, ...data }) => {
			try {
				const response = await axios.put(
					getDiscountCategoryByIdEndpoint(id),
					data
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
