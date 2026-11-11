import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_DISCOUNTS_ORDER_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import { DiscountOrderData } from '@/types/discounts';

export const useUpdateDiscountOrder = (
	options?: UseMutationOptions<
		void,
		AxiosError<ServerResponseError>,
		DiscountOrderData
	>
) =>
	useMutation({
		mutationFn: async (data) => {
			try {
				const response = await axios.put(
					GET_DISCOUNTS_ORDER_API_ENDPOINT,
					data
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
